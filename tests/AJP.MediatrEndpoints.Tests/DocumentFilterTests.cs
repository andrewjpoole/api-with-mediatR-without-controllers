using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using AJP.MediatrEndpoints.Tests.TestRequestHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace AJP.MediatrEndpoints.Tests
{
    public class DocumentFilterTests
    {
        private EndpointMetadataDecoratorAttribute _decorator;
        private Endpoint _endpoint;
        private OpenApiDocument _document;
        private SchemaRepository _schemaRepository;
        private Mock<ISchemaGenerator> _mockSchemaGenerator;
        private DocumentFilterContext _context;
        private Mock<EndpointDataSource> _mockEndpointDataSource;
        private Mock<IOpenApiOperationRenderer> _mockOperationRenderer;
        private AddEndpointsDocumentFilter _sut;

        public DocumentFilterTests()
        {
            _decorator = new EndpointMetadataDecoratorAttribute();
            _endpoint = new Endpoint(null, new EndpointMetadataCollection( new List<object>{_decorator}), "displayName");
            _document = new OpenApiDocument{
                Paths = new OpenApiPaths()
            };

            _schemaRepository = new SchemaRepository();
            _mockSchemaGenerator = new Mock<ISchemaGenerator>();

            _mockOperationRenderer = new Mock<IOpenApiOperationRenderer>();
            _mockOperationRenderer.Setup(r => r.Render(
                It.IsAny<Endpoint>(), 
                It.IsAny<EndpointMetadataDecoratorAttribute>(), 
                It.IsAny<OpenApiDocument>(), 
                It.IsAny<DocumentFilterContext>()))
                .Returns(new OpenApiOperation
                    {
                        Parameters = new List<OpenApiParameter>()
                    });
            
            _context =
                new DocumentFilterContext(new ApiDescription[] { }, _mockSchemaGenerator.Object, _schemaRepository);
            
            _mockEndpointDataSource = new Mock<EndpointDataSource>();
        }
        
        [Fact]
        public void Apply_should_continue_if_decorator_is_null_leaving_document_paths_empty()
        {
            _decorator = null;
            
            _mockEndpointDataSource.Setup(e => e.Endpoints).Returns(new Collection<Endpoint>
            {
                new Endpoint(null, new EndpointMetadataCollection( new List<object>()), "displayName")
            });

            _sut = new AddEndpointsDocumentFilter(_mockEndpointDataSource.Object, _mockOperationRenderer.Object);

            _sut.Apply(_document, _context);

            _mockOperationRenderer.Verify(r => r.Render(
                It.IsAny<Endpoint>(),
                It.IsAny<EndpointMetadataDecoratorAttribute>(),
                It.IsAny<OpenApiDocument>(),
                It.IsAny<DocumentFilterContext>()), Times.Never);
            
            _document.Paths.Count.Should().Be(0);
        }
        
        [Fact]
        public void Apply_should_add_a_path_swaggerDoc_Paths_doesnt_contain_the_key()
        {
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                Pattern = "/test",
                RequestType = typeof(TestRequestPlain),
                ResponseType = typeof(TestResponse),
            };

            _mockEndpointDataSource.Setup(e => e.Endpoints).Returns(new Collection<Endpoint>
            {
                new Endpoint(null, new EndpointMetadataCollection( new List<object>{_decorator}), "displayName")
            });
            
            _sut = new AddEndpointsDocumentFilter(_mockEndpointDataSource.Object, _mockOperationRenderer.Object);

            _sut.Apply(_document, _context);
            
            _document.Paths.Count.Should().Be(1);
        }
        
        [Fact]
        public void Apply_should_add_to_an_existing_path_swaggerDoc_Paths_already_contains_the_key()
        {
            _document.Paths.Add("/test", new OpenApiPathItem());
            
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                Pattern = "/test",
                RequestType = typeof(TestRequestPlain),
                ResponseType = typeof(TestResponse),
            };

            _mockEndpointDataSource.Setup(e => e.Endpoints).Returns(new Collection<Endpoint>
            {
                new Endpoint(null, new EndpointMetadataCollection( new List<object>{_decorator}), "displayName")
            });
            
            _sut = new AddEndpointsDocumentFilter(_mockEndpointDataSource.Object, _mockOperationRenderer.Object);

            _sut.Apply(_document, _context);
            
            _document.Paths.Count.Should().Be(1);
        }
    }
}