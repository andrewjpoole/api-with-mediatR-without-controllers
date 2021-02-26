using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public static class AdditionalParameter 
    {
        public enum In 
        {
            Route,
            Query,
            Header
        }

        public static List<OpenApiParameter> NewDictionary() => new List<OpenApiParameter>();

        public static List<OpenApiParameter> AddStringParam(this List<OpenApiParameter> additionalParameters, 
            string name, In location, bool required = true, string description = "", bool deprecated = false) => 
            AddParam(additionalParameters, "string", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddIntegerParam(this List<OpenApiParameter> additionalParameters,
            string name, In location, bool required = true, string description = "", bool deprecated = false) =>
            AddParam(additionalParameters, "integer", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddBoolParam(this List<OpenApiParameter> additionalParameters, 
            string name, In location, bool required = true, string description = "", bool deprecated = false) => 
                AddParam(additionalParameters, "boolean", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddEnumParam(this List<OpenApiParameter> additionalParameters, string name, Type enumType,  In location, bool required = true, string description = "", bool deprecated = false)
        {
            var enumNames = Enum.GetNames(enumType).ToList().Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
            return AddParam(additionalParameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = "string", Enum = enumNames },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });
        }

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> additionalParameters, string type, string name, In location, bool required = true, string description = "", bool deprecated = false) =>
            AddParam(additionalParameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = type },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> additionalParameters, OpenApiParameter parameter)
        {
            additionalParameters ??= NewDictionary();
            additionalParameters.Add(parameter);
            return additionalParameters;
        }

        private static ParameterLocation ConvertParamType(In type) =>
        type switch
        {
            In.Route => ParameterLocation.Path,
            In.Header => ParameterLocation.Header,
            _ => ParameterLocation.Query,
        };
    }
}
