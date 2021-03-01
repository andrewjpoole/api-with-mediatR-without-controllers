using System;
using System.Collections;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AJP.MediatrEndpoints.PropertyAttributes;
using Microsoft.OpenApi.Any;

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
                var swaggerDecorator = endpoint.Metadata.GetMetadata<SwaggerEndpointDecoraterAttribute>();
                if (swaggerDecorator == null)
                    continue;
                
                var requestSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorator.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorator.ResponseType, context.SchemaRepository);

                var bodyExampeObject = JsonDocument.Parse("{}").RootElement;
                
                // Get properties on requestType
                var propsLeftForBody = 0;
                var requestTypeProps =
                    swaggerDecorator.RequestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var requestTypePropParamDefinitions = new List<OpenApiParameter>();
                foreach(var requestTypeProp in requestTypeProps)
                {
                    var isRouteParam = requestTypeProp.GetCustomAttributes(typeof(RouteParameterAttribute)).Any();
                    var isHeaderParam = requestTypeProp.GetCustomAttributes(typeof(HeaderParameterAttribute)).Any();
                    var isOptionalParam = requestTypeProp.GetCustomAttributes(typeof(OptionalPropertyAttribute)).Any();
                    var swaggerDescription = (SwaggerDescriptionAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerDescriptionAttribute)).FirstOrDefault();
                    var swaggerExample = (SwaggerExampleAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerExampleAttribute)).FirstOrDefault();

                    // Render props to the body unless instructed to use route or header OR if GET
                    if (!isRouteParam && !isHeaderParam && swaggerDecorator.OperationType != OperationType.Get)
                    {
                        var exampleValue = swaggerExample?.Example ?? GetJsonSchemaTypeString(requestTypeProp);
                        bodyExampeObject = bodyExampeObject.AddProperty(requestTypeProp.Name, exampleValue);
                        propsLeftForBody += 1;
                        continue;
                    }
                    
                    // if not route or header, add property to json object for the body example
                    
                    
                    // is enum?
                    if (requestTypeProp.PropertyType == typeof(Enum))
                    {
                        
                    }

                    requestTypePropParamDefinitions.Add(new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = requestTypeProp.Name,
                        Schema = new OpenApiSchema { Type = GetJsonSchemaTypeString(requestTypeProp) },
                        Required = !isOptionalParam,
                        Deprecated = swaggerDescription?.Deprecated ?? false,
                        Description = swaggerDescription?.Description
                    });
                }
                
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
                    swaggerDoc.Tags = new List<OpenApiTag> 
                    {
                        new OpenApiTag
                        { 
                            Name = swaggerDecorator.EndpointGroupName, 
                            Description = swaggerDecorator.EndpointGroupDescription 
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
                            Name = swaggerDecorator.EndpointGroupName // confusingly, this seems to be what groups operations together into a path.
                        } 
                    },                    
                    Description = swaggerDecorator.SwaggerOperationDescription,
                    Parameters = new List<OpenApiParameter>(),
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

                if (propsLeftForBody > 0)
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                "default", new OpenApiMediaType
                                {
                                    //Schema = requestSchema
                                    Schema = new OpenApiSchema
                                    {
                                        Default = new OpenApiString(bodyExampeObject.GetRawText()),
                                    }
                                }
                            }
                        }
                    };

                foreach (var additionalParameter in requestTypePropParamDefinitions)
                {
                    operation.Parameters.Add(additionalParameter);
                }
                
                if (swaggerDecorator.AdditionalParameterDefinitions != null)
                {                
                    foreach (var additionalParameter in swaggerDecorator.AdditionalParameterDefinitions)
                    {
                        operation.Parameters.Add(additionalParameter);
                    }
                }

                pathItem.Operations.Add(swaggerDecorator.OperationType, operation);
            }
        }

        private string GetJsonSchemaTypeString(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            if (new List<Type> {typeof(string), typeof(DateTime)}.Contains(type))
                return "string";
            if (new List<Type> {typeof(int), typeof(float), typeof(double), typeof(decimal)}.Contains(type))
                return "number";
            if (type == typeof(bool))
                return "bool";
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return "array";
            return "object";
        }
    }
}
