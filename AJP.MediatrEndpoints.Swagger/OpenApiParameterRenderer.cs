using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Swagger.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.Swagger
{
    public class OpenApiParameterRenderer : IOpenApiParameterRenderer
    {
        public (IEnumerable<OpenApiParameter> Parameters, JsonElement bodyExampleObject) Render(EndpointMetadataDecoratorAttribute swaggerDecorator)
        {
            var parameters = new List<OpenApiParameter>();
            var routeParamNames = GetRouteParamNamesFromPattern(swaggerDecorator.Pattern);

            // Get an empty jon object, which will have any TRequest properties that are not route/query/header and therefore expected on the body
            var bodyExampleObject = JsonDocument.Parse("{}").RootElement;
            
            var requestTypeProps = swaggerDecorator.RequestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var requestTypeProp in requestTypeProps)
            {
                // Check if parameter is overriden by the Parameters dictionary on the swaggerDecorator
                if (swaggerDecorator.OverrideParameterDefinitions.ContainsKey(requestTypeProp.Name))
                {
                    parameters.Add(swaggerDecorator.OverrideParameterDefinitions[requestTypeProp.Name]);
                    continue;
                }
                
                //var isRouteParam = requestTypeProp.GetCustomAttributes(typeof(SwaggerRouteParameterAttribute)).Any();
                //var isHeaderParam = requestTypeProp.GetCustomAttributes(typeof(SwaggerHeaderParameterAttribute)).Any();
                var isOptionalParam = requestTypeProp.GetCustomAttributes(typeof(OptionalPropertyAttribute)).Any();
                var swaggerDescription = (SwaggerDescriptionAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerDescriptionAttribute)).FirstOrDefault();
                var swaggerExample = (SwaggerExampleValueAttribute)requestTypeProp.GetCustomAttributes(typeof(SwaggerExampleValueAttribute)).FirstOrDefault();

                var location = GetParameterLocation(requestTypeProp, routeParamNames);
                
                // Render the property to the body unless instructed to use route/header/query
                // OR if operation is a GET (not allowed a body in most browsers)
                if (location == ParameterLocation.Query && swaggerDecorator.OperationType != OperationType.Get)
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
                
                parameters.Add(new OpenApiParameter
                {
                    In = location,
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

            return (parameters, bodyExampleObject);
        }
        
        private static string GetJsonSchemaTypeString(PropertyInfo propertyInfo)
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
        
        private static ParameterLocation GetParameterLocation(MemberInfo prop, ICollection<string> routeParamNames)
        {
            var isRouteParam = prop.GetCustomAttributes(typeof(SwaggerRouteParameterAttribute)).Any();
            var isHeaderParam = prop.GetCustomAttributes(typeof(SwaggerHeaderParameterAttribute)).Any();

            if (routeParamNames.Contains(prop.Name))
            {
                isRouteParam = true;
            }

            if (isHeaderParam)
                return ParameterLocation.Header;

            if (isRouteParam)
                return ParameterLocation.Path;

            return ParameterLocation.Query;
        }
        
        private static object GetJsonExampleValue(PropertyInfo propertyInfo)
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
        
        public static List<string> GetRouteParamNamesFromPattern(string pattern)
        {
            var regexPattern = @"(?<=\{)[^}]*(?=\})";
            
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            var matches = regex.Matches(pattern);

            return matches.Select(x => x.Value).ToList();
        }
    }
}