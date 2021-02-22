using Microsoft.OpenApi.Models;
using System;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerDecoraterAttribute : Attribute
    {
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public OperationType OperationType { get; }

        public SwaggerDecoraterAttribute(Type requestType, Type responseType, OperationType operationType)
        {
            RequestType = requestType;
            ResponseType = responseType;
            OperationType = operationType;
        }
    }    
}
