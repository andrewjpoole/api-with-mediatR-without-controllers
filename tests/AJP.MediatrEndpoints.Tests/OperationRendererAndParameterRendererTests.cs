using System.Collections.Generic;
using System.Linq;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using AJP.MediatrEndpoints.Swagger.Attributes;
using AJP.MediatrEndpoints.Tests.TestRequestHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;
using Xunit.Sdk;

namespace AJP.MediatrEndpoints.Tests
{
    public class OperationRendererAndParameterRendererTests
    {
        private EndpointMetadataDecoratorAttribute _decorator;
        private Endpoint _endpoint;
        private OpenApiDocument _document;
        private SchemaRepository _schemaRepository;
        private Mock<ISchemaGenerator> _mockSchemaGenerator;
        private DocumentFilterContext _context;
        private OpenApiOperationRenderer _operationRenderer;

        public OperationRendererAndParameterRendererTests()
        {
            _decorator = new EndpointMetadataDecoratorAttribute();
            _endpoint = new Endpoint(null, new EndpointMetadataCollection( new List<object>{_decorator}), "displayName");
            _document = new OpenApiDocument();

            _schemaRepository = new SchemaRepository();
            _mockSchemaGenerator = new Mock<ISchemaGenerator>();
            
            _context =
                new DocumentFilterContext(new ApiDescription[] { }, _mockSchemaGenerator.Object, _schemaRepository);

            _operationRenderer = new OpenApiOperationRenderer(new OpenApiParameterRenderer());
        }
        
        [Fact]
        public void Render_should_render_OpenApiOperation_with_no_OpenApiParameters_on_the_body_for_GET()
        {
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                OperationType = OperationType.Get,
                Pattern = "/test",
                RequestType = typeof(TestRequestWithAttribute),
                ResponseType = typeof(TestResponse),
                SuccessfulStatusCode = 200
            };
            
            var result = _operationRenderer.Render(_endpoint, _decorator, _document, _context);

            result.Should().NotBeNull();
            result.Description.Should().Be("testDescriptionFromAttribute");
            result.Responses.Count.Should().Be(1);
            result.Responses.First().Key.Should().Be("200");
            
            result.Parameters.Count.Should().Be(8);
            
            result.Parameters.First().Name.Should().Be("Prop1");
            result.Parameters.First().In.Should().Be(ParameterLocation.Header);
            
            result.Parameters.Skip(1).First().Name.Should().Be("Prop2");
            result.Parameters.Skip(1).First().In.Should().Be(ParameterLocation.Query);
            
            result.Parameters.Skip(2).First().Name.Should().Be("Prop3");
            result.Parameters.Skip(2).First().In.Should().Be(ParameterLocation.Path);
            result.Parameters.Skip(2).First().Description.Should().Be("Popp3DescriptionFromAttribute");
            ((OpenApiString)result.Parameters.Skip(2).First().Schema.Example).Value.Should().Be("Prop3ExampleFromAttribute");
            
            result.Parameters.Skip(3).First().Name.Should().Be("Prop4");
            result.Parameters.Skip(3).First().In.Should().Be(ParameterLocation.Query);
            
            result.Parameters.Skip(4).First().Name.Should().Be("Prop5");
            result.Parameters.Skip(4).First().In.Should().Be(ParameterLocation.Query);
            ((OpenApiString)result.Parameters.Skip(4).First().Schema.Enum[0]).Value.Should().Be("Red");
            ((OpenApiString)result.Parameters.Skip(4).First().Schema.Enum[1]).Value.Should().Be("Blue");
            ((OpenApiString)result.Parameters.Skip(4).First().Schema.Enum[2]).Value.Should().Be("Green");

        }
        
        [Fact]
        public void Render_should_render_OpenApiOperation_with_all_non_route_and_headers_OpenApiParameters_on_the_body_for_POST()
        {
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                OperationType = OperationType.Post,
                Pattern = "/test",
                RequestType = typeof(TestRequestWithAttribute),
                ResponseType = typeof(TestResponse),
                SuccessfulStatusCode = 200
            };
            
            var result = _operationRenderer.Render(_endpoint, _decorator, _document, _context);

            result.Should().NotBeNull();
            result.Description.Should().Be("testDescriptionFromAttribute");
            result.Responses.Count.Should().Be(1);
            result.Responses.First().Key.Should().Be("200");
            
            result.Parameters.Count.Should().Be(2);
            
            result.Parameters.First().Name.Should().Be("Prop1");
            result.Parameters.First().In.Should().Be(ParameterLocation.Header);
            
            result.Parameters.Skip(1).First().Name.Should().Be("Prop3");
            result.Parameters.Skip(1).First().In.Should().Be(ParameterLocation.Path);
            result.Parameters.Skip(1).First().Description.Should().Be("Popp3DescriptionFromAttribute");
            ((OpenApiString)result.Parameters.Skip(1).First().Schema.Example).Value.Should().Be("Prop3ExampleFromAttribute");
            
            // rest of the props should be rendered on the request body
            var requestBody = result.RequestBody.Content.First().Value.Schema;
            ((OpenApiString)requestBody.Default).Value.Should().Be("{\"prop8\":\"{}\",\"prop7\":\"string\",\"prop6\":true,\"prop5\":\"{}\",\"prop4\":\"[]\",\"prop2\":123}");
        }
        
        [Fact]
        public void Render_should_render_OpenApiOperation_with_OpenApiParameters_in_the_route_if_name_matches_variable_in_pattern()
        {
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                OperationType = OperationType.Post,
                Pattern = "/test/{Prop4}",
                RequestType = typeof(TestRequestWithAttribute),
                ResponseType = typeof(TestResponse)
            };
            
            var result = _operationRenderer.Render(_endpoint, _decorator, _document, _context);

            result.Should().NotBeNull();
            result.Parameters.First(p => p.Name == "Prop4").In.Should().Be(ParameterLocation.Path);
        }
        
        [Fact]
        public void Render_should_render_OpenApiOperation_with_OpenApiParameter_overriden_decorator()
        {
            _decorator = new EndpointMetadataDecoratorAttribute
            {
                OperationType = OperationType.Post,
                Pattern = "/test/",
                RequestType = typeof(TestRequestWithAttribute),
                ResponseType = typeof(TestResponse),
                OverrideParameterDefinitions = new Dictionary<string, OpenApiParameter>
                {
                    {"Prop4", new OpenApiParameter
                        {
                            Name = "Prop4",
                            In = ParameterLocation.Header
                        }
                    }
                }
            };
            
            var result = _operationRenderer.Render(_endpoint, _decorator, _document, _context);

            result.Should().NotBeNull();
            result.Parameters.First(p => p.Name == "Prop4").In.Should().Be(ParameterLocation.Header);
        }
    }

    [SwaggerDescription("testDescriptionFromAttribute")]
    public class TestRequestWithAttribute
    {
        [SwaggerHeaderParameter]
        public string Prop1 { get; set; }
        
        [SwaggerQueryParameter]
        public int Prop2 { get; set; }
        
        [SwaggerRouteParameter]
        [SwaggerDescription("Popp3DescriptionFromAttribute")]
        [SwaggerExampleValue("Prop3ExampleFromAttribute")]
        public bool Prop3 { get; set; }
        
        public List<string> Prop4 { get; set; }
        public TestEnum Prop5 { get; set; }
        public bool Prop6 { get; set; }
        public string Prop7 { get; set; }
        public AnotherClass Prop8 { get; set; }
    }

    public class AnotherClass
    {
        public string Prop1 { get; set; }
    }
}