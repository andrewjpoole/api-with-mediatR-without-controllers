using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Routing;
using AJP.MediatrEndpoints;
using System.Collections.Generic;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class AddEndpointsDocumentFilter : IDocumentFilter
    {
        private readonly EndpointDataSource _endpointDataSource;

        public AddEndpointsDocumentFilter(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var endpoint in _endpointDataSource.Endpoints)
            {
                var swaggerDecorater = endpoint.Metadata.GetMetadata<SwaggerDecoraterAttribute>();
                if (swaggerDecorater == null)
                    continue;

                var requestSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.ResponseType, context.SchemaRepository);

                // Get existing path to add operation to, or create a new one
                OpenApiPathItem pathItem;
                if (swaggerDoc.Paths.ContainsKey(endpoint.DisplayName))
                {
                    pathItem = swaggerDoc.Paths[endpoint.DisplayName];
                }
                else 
                {
                    pathItem = new OpenApiPathItem();                    
                    swaggerDoc.Paths.Add(endpoint.DisplayName, pathItem);
                }
                              
                // Add the operation
                var operation = new OpenApiOperation 
                {
                    Description = $"operation for {swaggerDecorater.OperationType} {endpoint.DisplayName}",
                    Parameters = new List<OpenApiParameter> 
                    {
                        new OpenApiParameter 
                        {
                            In = ParameterLocation.Path,
                            Name = "exampleParam",
                            Schema = requestSchema
                        }
                    },
                    RequestBody = new OpenApiRequestBody 
                    {
                        Content = new Dictionary<string, OpenApiMediaType> 
                        {
                            {"default", new OpenApiMediaType 
                                {
                                    Schema = requestSchema        
                                }
                            }
                        }
                    },
                    Responses = new OpenApiResponses 
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {"default", new OpenApiMediaType
                                    {
                                        Schema = responseSchema
                                    }
                                }
                            }
                        }
                    }
                };

                // TODO figure out how to add any path and query parameters which might exist?

                pathItem.Operations.Add(swaggerDecorater.OperationType, operation);
            }
        }
    }
}
