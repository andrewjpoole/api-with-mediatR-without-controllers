using System.Collections.Generic;
using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public class EndpointGroupMapper
    {
        private readonly IEndpointRouteBuilder _endpoints;
        private readonly string _path;
        private readonly string _name;
        private readonly string _description;
        
        public EndpointGroupMapper(IEndpointRouteBuilder endpoints, string path, string name, string description)
        {
            _endpoints = endpoints;
            _path = path;
            _name = name;
            _description = description;
        }        

        public EndpointGroupMapper WithGet<TRequest, TResponse>(string pattern, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Get, pattern, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithPost<TRequest, TResponse>(string pattern, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Post, pattern, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithPut<TRequest, TResponse>(string pattern, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Put, pattern, additionalParameterDefinitions);
            return this;
        }

        public EndpointGroupMapper WithDelete<TRequest, TResponse>(string pattern, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Delete, pattern, additionalParameterDefinitions);
            return this;
        }   

        private void AddOperation<TRequest, TResponse>(OperationType operationType, string pattern, List<OpenApiParameter> additionalParameterDefinitions = null)
        {      
            var method = new[] { operationType.ToString().ToUpper() };
            var fullPattern = $"{_path}{pattern}";
            _endpoints.MapMethods(fullPattern, method, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
            .WithMetadata(
                new SwaggerEndpointDecoratorAttribute
                {
                    EndpointGroupPath = _path,
                    EndpointGroupName = _name,
                    EndpointGroupDescription = _description,
                    Pattern = fullPattern,
                    OperationType = operationType,
                    RequestType = typeof(TRequest),
                    ResponseType = typeof(TResponse),
                    AdditionalParameterDefinitions = additionalParameterDefinitions
                });
        }
    }
}
