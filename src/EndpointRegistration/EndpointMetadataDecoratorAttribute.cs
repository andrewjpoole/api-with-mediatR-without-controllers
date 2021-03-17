using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public class EndpointMetadataDecoratorAttribute : Attribute
    {
        public string Pattern { get; init; }
        public Type RequestType { get; init; }
        public Type ResponseType { get; init; }
        public OperationType OperationType { get; init; }
        public string EndpointGroupName { get; init; }
        public string EndpointGroupPath { get; init; }
        public string EndpointGroupDescription { get; init; }
        public List<OpenApiParameter> OverrideParameterDefinitions { get; init; } = new List<OpenApiParameter>();
        public int SuccessfulStatusCode { get; init; }
        public string Description { get; set; }
    }
}
