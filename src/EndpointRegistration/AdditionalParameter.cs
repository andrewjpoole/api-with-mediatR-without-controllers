using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace AJP.MediatrEndpoints
{
    public static class AdditionalParameter 
    {
        public enum In 
        {
            Route,
            Query,
            Header
        }

        public static List<OpenApiParameter> NewDictionary()
        {
            return new List<OpenApiParameter>();
        }

        public static List<OpenApiParameter> AddStringParam(this List<OpenApiParameter> additionalParameters, string name, In location, bool required = true, string description = "", bool deprecated = false) 
        {
            return AddParam(additionalParameters, "string", name, location, required, description, deprecated);
        }

        public static List<OpenApiParameter> AddIntegerParam(this List<OpenApiParameter> additionalParameters, string name, In location, bool required = true, string description = "", bool deprecated = false)
        {
            return AddParam(additionalParameters, "integer", name, location, required, description, deprecated);
        }

        public static List<OpenApiParameter> AddBoolParam(this List<OpenApiParameter> additionalParameters, string name, In location, bool required = true, string description = "", bool deprecated = false)
        {
            return AddParam(additionalParameters, "boolean", name, location, required, description, deprecated);
        }

        public static List<OpenApiParameter> AddEnumParam(this List<OpenApiParameter> additionalParameters, string name, Type enumType,  In location, bool required = true, string description = "", bool deprecated = false)
        {
            var openApiEnumValues = new List<IOpenApiAny>();
            var enumValues = Enum.GetValues(enumType); //.Select(x => new OpenApiString(x)).ToList();
            foreach (var enumValue in enumValues)
            {
                openApiEnumValues.Add(new OpenApiString(enumValue.ToString()));
            }

            return AddParam(additionalParameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = "string", Enum = openApiEnumValues },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });
        }

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> additionalParameters, string type, string name, In location, bool required = true, string description = "", bool deprecated = false) 
        {
            return AddParam(additionalParameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = type },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });
        }

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> additionalParameters, OpenApiParameter parameter)
        {
            if (additionalParameters is null)
                additionalParameters = NewDictionary();

            additionalParameters.Add(parameter);

            return additionalParameters;
        }

        private static ParameterLocation ConvertParamType(In type) 
        {
            return type switch
            {
                In.Route => ParameterLocation.Path,
                In.Header => ParameterLocation.Header,
                _ => ParameterLocation.Query,
            };
        }
    }
}
