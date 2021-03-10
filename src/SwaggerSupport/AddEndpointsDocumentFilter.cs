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
            swaggerDoc.Tags = new List<OpenApiTag>();
            foreach (var endpoint in _endpointDataSource.Endpoints)
            {
                var swaggerDecorator = endpoint.Metadata.GetMetadata<SwaggerEndpointDecoratorAttribute>();
                if (swaggerDecorator == null)
                    continue;
                
                //var requestSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorator.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorator.ResponseType, context.SchemaRepository);
                var endpointSwaggerDescription =
                    (SwaggerDescriptionAttribute)swaggerDecorator.RequestType.GetCustomAttributes(typeof(SwaggerDescriptionAttribute)).FirstOrDefault();

                // Get an empty json element to add body parameters to
                var bodyExampleObject = JsonDocument.Parse("{}").RootElement;
                
                // Get properties on requestType
                var requestTypeProps = swaggerDecorator.RequestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var requestTypePropParamDefinitions = new List<OpenApiParameter>();
                foreach(var requestTypeProp in requestTypeProps)
                {
                    var isQueryParam = requestTypeProp.GetCustomAttributes(typeof(SwaggerQueryParameterAttribute)).Any();
                    var isRouteParam = requestTypeProp.GetCustomAttributes(typeof(SwaggerRouteParameterAttribute)).Any();
                    var isHeaderParam = requestTypeProp.GetCustomAttributes(typeof(SwaggerHeaderParameterAttribute)).Any();
                    var isOptionalParam = requestTypeProp.GetCustomAttributes(typeof(OptionalPropertyAttribute)).Any();
                    var swaggerDescription = (SwaggerDescriptionAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerDescriptionAttribute)).FirstOrDefault();
                    var swaggerExample = (SwaggerExampleValueAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerExampleValueAttribute)).FirstOrDefault();

                    // Render props to the body unless instructed to use route or header OR if GET
                    if (!isRouteParam && !isHeaderParam && swaggerDecorator.OperationType != OperationType.Get)
                    {
                        var exampleValue = swaggerExample?.Example ?? GetJsonExampleValue(requestTypeProp);
                        var propName = JsonNamingPolicy.CamelCase.ConvertName(requestTypeProp.Name);
                        bodyExampleObject = bodyExampleObject.AddProperty(propName, exampleValue);
                        continue;
                    }
                    
                    // If propertyType is an enum then render the names
                    OpenApiSchema enumSchema = null;
                    if (requestTypeProp.PropertyType.IsEnum)
                    {
                        var enumNames = Enum.GetNames(requestTypeProp.PropertyType).ToList().Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                        enumSchema = new OpenApiSchema {Type = "string", Enum = enumNames};
                    }
                    
                    requestTypePropParamDefinitions.Add(new OpenApiParameter
                    {
                        In = GetParameterLocation(isQueryParam, isRouteParam, isHeaderParam),
                        Name = requestTypeProp.Name,
                        Schema = enumSchema ?? new OpenApiSchema
                        {
                            Type = GetJsonSchemaTypeString(requestTypeProp), 
                            Example = new OpenApiString(swaggerExample?.Example)
                        },
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
                    swaggerDoc.Tags.Add(new()
                    { 
                        Name = swaggerDecorator.EndpointGroupName, 
                        Description = swaggerDecorator.EndpointGroupDescription
                    });
                }
                              
                // Add the operation
                var operation = new OpenApiOperation 
                {                    
                    Tags = new List<OpenApiTag> 
                    { 
                        new()
                        { 
                            Name = swaggerDecorator.EndpointGroupName // this groups operations together into a path.
                        } 
                    },                    
                    Description = endpointSwaggerDescription?.Description,
                    Parameters = new List<OpenApiParameter>(),
                    Responses = new OpenApiResponses 
                    {
                        ["200"] = new()
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

                // Add a body if there are any props left (not already rendered as route, query or header)
                if (bodyExampleObject.EnumerateObject().Any())
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                "default", new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Default = new OpenApiString(bodyExampleObject.GetRawText()),
                                    }
                                }
                            }
                        }
                    };

                // Add the route, query and header parameters to the operation
                foreach (var additionalParameter in requestTypePropParamDefinitions)
                    operation.Parameters.Add(additionalParameter);
                
                if (swaggerDecorator.AdditionalParameterDefinitions != null)
                    foreach (var additionalParameter in swaggerDecorator.AdditionalParameterDefinitions)
                        operation.Parameters.Add(additionalParameter);

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

        private object GetJsonExampleValue(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            if (new List<Type> {typeof(string), typeof(DateTime)}.Contains(type))
                return "string";
            if (new List<Type> {typeof(int), typeof(float), typeof(double), typeof(decimal)}.Contains(type))
                return 123;
            if (type == typeof(bool))
                return true;
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return "[]";
            return "{}";
        }
        
        private ParameterLocation GetParameterLocation(bool isQuery, bool isRoute, bool isHeader)
        {
            if (isHeader)
                return ParameterLocation.Header;
            if (isRoute)
                return ParameterLocation.Path;

            return ParameterLocation.Query;
        }
    }
}
