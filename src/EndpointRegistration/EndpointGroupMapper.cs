using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace AJP.MediatrEndpoints
{
    public class EndpointGroupMapper
    {
        private readonly IEndpointRouteBuilder endpoints;
        private readonly string path;
        private readonly string name;
        private readonly string description;
        
        public EndpointGroupMapper(IEndpointRouteBuilder endpoints, string path, string name, string description)
        {
            this.endpoints = endpoints;
            this.path = path;
            this.name = name;
            this.description = description;
        }        

        public EndpointGroupMapper WithGet<TRequest, TResponse>(string pattern, string swaggerOperationDescription = "", List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Get, pattern, swaggerOperationDescription, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithPost<TRequest, TResponse>(string pattern, string swaggerOperationDescription = "", List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Post, pattern, swaggerOperationDescription, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithPut<TRequest, TResponse>(string pattern, string swaggerOperationDescription = "", List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Put, pattern, swaggerOperationDescription, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithDelete<TRequest, TResponse>(string pattern, string swaggerOperationDescription = "", List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Delete, pattern, swaggerOperationDescription, additionalParameterDefinitions);
            return this;
        }   

        private void AddOperation<TRequest, TResponse>(OperationType operationType, string pattern, string swaggerOperationDescription = "", List<OpenApiParameter> additionalParameterDefinitions = null)
        {      
            var method = new[] { operationType.ToString().ToUpper() };
            var fullPattern = $"{path}{pattern}";
            endpoints.MapMethods(fullPattern, method, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
            .WithMetadata(
                new SwaggerEndpointDecoraterAttribute
                {
                    EndpointGroupPath = path,
                    EndpointGroupName = name,
                    EndpointGroupDescription = description,
                    Pattern = fullPattern,
                    OperationType = operationType,
                    RequestType = typeof(TRequest),
                    ResponseType = typeof(TResponse),
                    AdditionalParameterDefinitions = additionalParameterDefinitions
                });
        }
    }
}
