using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Routing;
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
                var swaggerDecorater = endpoint.Metadata.GetMetadata<SwaggerEndpointDecoraterAttribute>();
                if (swaggerDecorater == null)
                    continue;
                
                var requestSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.ResponseType, context.SchemaRepository);
                                
                // Get existing path to add operation to, or create a new one
                OpenApiPathItem pathItem;
                if (swaggerDoc.Paths.ContainsKey(swaggerDecorater.Pattern))
                {
                    pathItem = swaggerDoc.Paths[swaggerDecorater.Pattern];
                }
                else 
                {
                    pathItem = new OpenApiPathItem();
                    swaggerDoc.Paths.Add(swaggerDecorater.Pattern, pathItem);
                    swaggerDoc.Tags = new List<OpenApiTag> 
                    {
                        new OpenApiTag
                        { 
                            Name = swaggerDecorater.EndpointGroupName, 
                            Description = swaggerDecorater.EndpointGroupDescription 
                        }
                    };
                }
                              
                // Add the operation
                var operation = new OpenApiOperation 
                {                    
                    Tags = new List<OpenApiTag> 
                    { 
                        new OpenApiTag 
                        { 
                            Name = swaggerDecorater.EndpointGroupName // confusingly, this seems to be what groups operations together into a path.
                            //Description = $"description for {swaggerDecorater.EndpointGroup.Path}"
                        } 
                    },                    
                    Description = swaggerDecorater.SwaggerOperationDescription,
                    Parameters = new List<OpenApiParameter>(),
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

                if (swaggerDecorater.AdditionalParameterDefinitions != null)
                {                
                    foreach (var additionalParameter in swaggerDecorater.AdditionalParameterDefinitions)
                    {
                        operation.Parameters.Add(additionalParameter);
                    }
                }

                pathItem.Operations.Add(swaggerDecorater.OperationType, operation);
            }
        }
    }
}
