using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerEndpointDecoratorAttribute : Attribute
    {
        public string Pattern { get; init; }
        public Type RequestType { get; init; }
        public Type ResponseType { get; init; }
        public OperationType OperationType { get; init; }
        public string EndpointGroupName { get; init; }
        public string EndpointGroupPath { get; init; }
        public string EndpointGroupDescription { get; init; }
        public List<OpenApiParameter> AdditionalParameterDefinitions { get; init; } = new List<OpenApiParameter>();
    }
}
