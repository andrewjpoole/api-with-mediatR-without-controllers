using Microsoft.OpenApi.Models;
using System;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerEndpointDecoraterAttribute : Attribute
    {
        public string Pattern { get; }
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public OperationType OperationType { get; }
        public SwaggerDocumentationEndpointGroup EndpointGroup { get; set; }
        public string SwaggerOperationDescription { get; set; }

        public SwaggerEndpointDecoraterAttribute(
            string pattern,
            Type requestType, 
            Type responseType, 
            OperationType operationType, 
            SwaggerDocumentationEndpointGroup endpointGroup, 
            string operationDescription = "")
        {
            EndpointGroup = endpointGroup; // basically path data
            Pattern = pattern;
            RequestType = requestType;
            ResponseType = responseType;
            OperationType = operationType;
            SwaggerOperationDescription = operationDescription;
        }
    }
}
