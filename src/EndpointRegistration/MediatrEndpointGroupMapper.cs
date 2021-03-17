using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public class MediatrEndpointGroupMapper
    {
        private readonly IEndpointRouteBuilder _endpoints;
        private readonly string _path;
        private readonly string _name;
        private readonly string _description;
        
        public MediatrEndpointGroupMapper(IEndpointRouteBuilder endpoints, string path, string name, string description = "")
        {
            _endpoints = endpoints;
            _path = AddLeadingSlashIfNotPresent(path);
            _name = name;
            _description = description;
        }        

        public MediatrEndpointGroupMapper WithGet<TRequest, TResponse>(string pattern, string description = "", int successfulStatusCode = 200, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Get, pattern, description, successfulStatusCode, additionalParameterDefinitions);
            return this;
        }

        public MediatrEndpointGroupMapper WithPost<TRequest, TResponse>(string pattern, string description = "", int successfulStatusCode = 200, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Post, pattern, description, successfulStatusCode, additionalParameterDefinitions);
            return this;
        }

        public MediatrEndpointGroupMapper WithPut<TRequest, TResponse>(string pattern, string description = "", int successfulStatusCode = 200, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Put, pattern, description, successfulStatusCode, additionalParameterDefinitions);
            return this;
        }

        public MediatrEndpointGroupMapper WithDelete<TRequest, TResponse>(string pattern, string description = "", int successfulStatusCode = 200, List<OpenApiParameter> additionalParameterDefinitions = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Delete, pattern, description, successfulStatusCode, additionalParameterDefinitions);
            return this;
        }   

        private void AddOperation<TRequest, TResponse>(OperationType operationType, string pattern, string description = "", int successfulStatusCode = 200, List<OpenApiParameter> additionalParameterDefinitions = null)
        {      
            var method = new[] { operationType.ToString().ToUpper() };
            var fullPattern = $"{_path}{AddLeadingSlashIfNotPresent(pattern)}";
            _endpoints.MapMethods(fullPattern, method, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>(successfulStatusCode))
            .WithMetadata(
                new EndpointMetadataDecoratorAttribute
                {
                    EndpointGroupPath = _path,
                    EndpointGroupName = _name,
                    EndpointGroupDescription = _description,
                    Pattern = fullPattern,
                    OperationType = operationType,
                    RequestType = typeof(TRequest),
                    ResponseType = typeof(TResponse),
                    Description = description,
                    SuccessfulStatusCode = successfulStatusCode,
                    OverrideParameterDefinitions = additionalParameterDefinitions
                });
        }

        private string AddLeadingSlashIfNotPresent(string path)
        {
            return !path.StartsWith("/") ? $"/{path}" : path;
        }
    }
}
