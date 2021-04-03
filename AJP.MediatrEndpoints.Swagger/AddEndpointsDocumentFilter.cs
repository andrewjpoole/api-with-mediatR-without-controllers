using System.Collections.Generic;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AJP.MediatrEndpoints.Swagger
{
    public class AddEndpointsDocumentFilter : IDocumentFilter
    {
        private readonly EndpointDataSource _endpointDataSource;
        private readonly IOpenApiOperationRenderer _openApiOperationRenderer;

        public AddEndpointsDocumentFilter(EndpointDataSource endpointDataSource, IOpenApiOperationRenderer openApiOperationRenderer)
        {
            _endpointDataSource = endpointDataSource;
            _openApiOperationRenderer = openApiOperationRenderer;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>();
            foreach (var endpoint in _endpointDataSource.Endpoints)
            {
                var swaggerDecorator = endpoint.Metadata.GetMetadata<EndpointMetadataDecoratorAttribute>();
                if (swaggerDecorator == null)
                    continue;

                var operation = _openApiOperationRenderer.Render(endpoint, swaggerDecorator, swaggerDoc, context);
                
                // Get existing path to add operation to, or create a new one
                OpenApiPathItem pathItem;
                if (swaggerDoc.Paths.ContainsKey(swaggerDecorator.Pattern))
                {
                    pathItem = swaggerDoc.Paths[swaggerDecorator.Pattern];
                }
                else 
                {
                    pathItem = new OpenApiPathItem();
                    swaggerDoc.Paths.Add(swaggerDecorator.Pattern, pathItem);
                    swaggerDoc.Tags.Add(new()
                    { 
                        Name = swaggerDecorator.EndpointGroupName, 
                        Description = swaggerDecorator.EndpointGroupDescription
                    });
                }
                
                // if (swaggerDecorator.OverrideParameterDefinitions != null)
                //     foreach (var additionalParameter in swaggerDecorator.OverrideParameterDefinitions)
                //         operation.Parameters.Add(additionalParameter);

                pathItem.Operations.Add(swaggerDecorator.OperationType, operation);
            }
        }
    }
}
