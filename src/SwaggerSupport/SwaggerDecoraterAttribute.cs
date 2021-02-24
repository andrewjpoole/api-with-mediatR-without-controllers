using Microsoft.OpenApi.Models;
using System;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerDecoraterAttribute : Attribute
    {
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public OperationType OperationType { get; }

        public string DisplayName { get; set; }
        public string Description { get; set; }

        public SwaggerDecoraterAttribute(Type requestType, Type responseType, OperationType operationType, string displayName, string description = "")
        {
            RequestType = requestType;
            ResponseType = responseType;
            OperationType = operationType;
            DisplayName = displayName;
            Description = description;
        }
    }    
}
