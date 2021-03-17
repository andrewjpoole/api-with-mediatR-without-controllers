using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.Swagger
{
    public static class ParameterDictionaryBuilder
    {
        public enum In 
        {
            Route,
            Query,
            Header
        }

        public static List<OpenApiParameter> NewDictionary() => new List<OpenApiParameter>();

        public static List<OpenApiParameter> AddStringParam(this List<OpenApiParameter> parameters, 
            string name, In location, bool required = true, string description = "", bool deprecated = false) => 
            AddParam(parameters, "string", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddIntegerParam(this List<OpenApiParameter> parameters,
            string name, In location, bool required = true, string description = "", bool deprecated = false) =>
            AddParam(parameters, "integer", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddBoolParam(this List<OpenApiParameter> parameters, 
            string name, In location, bool required = true, string description = "", bool deprecated = false) => 
                AddParam(parameters, "boolean", name, location, required, description, deprecated);

        public static List<OpenApiParameter> AddEnumParam(this List<OpenApiParameter> parameters, string name, Type enumType,  In location, bool required = true, string description = "", bool deprecated = false)
        {
            var enumNames = Enum.GetNames(enumType).ToList().Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
            return AddParam(parameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = "string", Enum = enumNames },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });
        }

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> parameters, string type, string name, In location, bool required = true, string description = "", bool deprecated = false) =>
            AddParam(parameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = type },
                Required = required,
                Deprecated = deprecated,
                Description = description
            });

        public static List<OpenApiParameter> AddParam(this List<OpenApiParameter> parameters, OpenApiParameter parameter)
        {
            parameters ??= NewDictionary();
            parameters.Add(parameter);
            return parameters;
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