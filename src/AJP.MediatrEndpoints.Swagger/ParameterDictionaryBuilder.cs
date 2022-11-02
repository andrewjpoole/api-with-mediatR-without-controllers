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

        public static Dictionary<string, OpenApiParameter> NewDictionary() => new();

        public static Dictionary<string, OpenApiParameter> AddStringParam(
            this Dictionary<string, OpenApiParameter> parameters, 
            string name, 
            In location, 
            bool required = true, 
            string description = "",
            string exampleValue = "", 
            bool deprecated = false
            ) => 
            AddParam(parameters, "string", name, location, required, description, exampleValue, deprecated);

        public static Dictionary<string, OpenApiParameter> AddIntegerParam(
            this Dictionary<string, OpenApiParameter> parameters,
            string name, 
            In location, 
            bool required = true, 
            string description = "", 
            string exampleValue = "",
            bool deprecated = false
            ) =>
            AddParam(parameters, "integer", name, location, required, description, exampleValue, deprecated);

        public static Dictionary<string, OpenApiParameter> AddBoolParam(
            this Dictionary<string, OpenApiParameter> parameters, 
            string name, 
            In location, 
            bool required = true, 
            string description = "", 
            string exampleValue = "",
            bool deprecated = false
            ) => 
                AddParam(parameters, "boolean", name, location, required, description, exampleValue, deprecated);

        public static Dictionary<string, OpenApiParameter> AddEnumParam(
            this Dictionary<string, OpenApiParameter> parameters, 
            string name, 
            Type enumType,  
            In location, 
            bool required = true, 
            string description = "", 
            string exampleValue = "",
            bool deprecated = false
            )
        {
            var enumNames = Enum.GetNames(enumType).ToList().Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
            return AddParam(parameters, new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema { Type = "string", Enum = enumNames },
                Required = required,
                Deprecated = deprecated,
                Description = description,
                Example = new OpenApiString(exampleValue)
            });
        }

        public static Dictionary<string, OpenApiParameter> AddParam(
            this Dictionary<string, OpenApiParameter> parameters, 
            string type, 
            string name, 
            In location, 
            bool required = true, 
            string description = "", 
            string exampleValue = "",
            bool deprecated = false
            )
        {
            var newParam = new OpenApiParameter
            {
                In = ConvertParamType(location),
                Name = name,
                Schema = new OpenApiSchema {Type = type},
                Required = required,
                Deprecated = deprecated,
                Description = description
            };
            
            if (!string.IsNullOrEmpty(exampleValue))
                newParam.Example = new OpenApiString(exampleValue);
            
            return AddParam(parameters, newParam);
        }

        public static Dictionary<string, OpenApiParameter> AddParam(this Dictionary<string, OpenApiParameter> parameters, OpenApiParameter parameter)
        {
            parameters ??= NewDictionary();
            parameters.Add(parameter.Name, parameter);
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