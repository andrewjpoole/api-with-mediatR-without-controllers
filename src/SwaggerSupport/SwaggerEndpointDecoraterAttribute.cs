using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerEndpointDecoraterAttribute : Attribute
    {
        public string Pattern { get; set; }
        public Type RequestType { get; set; }
        public Type ResponseType { get; set; }
        public OperationType OperationType { get; set; }
        public string EndpointGroupName { get; set; }
        public string EndpointGroupPath { get; set; }
        public string EndpointGroupDescription { get; set; }
        public string SwaggerOperationDescription { get; set; }
        public List<OpenApiParameter> AdditionalParameterDefinitions { get; internal set; } = new List<OpenApiParameter>();
    }
}
