using System;
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

        public MediatrEndpointGroupMapper WithGet<TRequest, TResponse>(
            string pattern, 
            string description = "", 
            int successfulStatusCode = 200, 
            Dictionary<string, OpenApiParameter> additionalParameterDefinitions = null,
            Action<IEndpointConventionBuilder> configureEndpoint = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Get, pattern, description, successfulStatusCode, additionalParameterDefinitions, configureEndpoint);
            return this;
        }

        public MediatrEndpointGroupMapper WithPost<TRequest, TResponse>(
            string pattern, 
            string description = "", 
            int successfulStatusCode = 200, 
            Dictionary<string, OpenApiParameter> additionalParameterDefinitions = null,
            Action<IEndpointConventionBuilder> configureEndpoint = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Post, pattern, description, successfulStatusCode, additionalParameterDefinitions, configureEndpoint);
            return this;
        }

        public MediatrEndpointGroupMapper WithPut<TRequest, TResponse>(
            string pattern, 
            string description = "", 
            int successfulStatusCode = 200, 
            Dictionary<string, OpenApiParameter> additionalParameterDefinitions = null,
            Action<IEndpointConventionBuilder> configureEndpoint = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Put, pattern, description, successfulStatusCode, additionalParameterDefinitions, configureEndpoint);
            return this;
        }

        public MediatrEndpointGroupMapper WithDelete<TRequest, TResponse>(
            string pattern, 
            string description = "", 
            int successfulStatusCode = 200, 
            Dictionary<string, OpenApiParameter> additionalParameterDefinitions = null,
            Action<IEndpointConventionBuilder> configureEndpoint = null)
        {
            AddOperation<TRequest, TResponse>(OperationType.Delete, pattern, description, successfulStatusCode, additionalParameterDefinitions, configureEndpoint);
            return this;
        }   

        private void AddOperation<TRequest, TResponse>(
            OperationType operationType, 
            string pattern, 
            string description = "", 
            int successfulStatusCode = 200, 
            Dictionary<string, OpenApiParameter> additionalParameterDefinitions = null, 
            Action<IEndpointConventionBuilder> configureEndpoint = null)
        {
            var method = new[] { operationType.ToString().ToUpper() };
            var fullPattern = $"{_path}{AddLeadingSlashIfNotPresent(pattern)}";
            var builder = _endpoints.MapMethods(fullPattern, method, MediatrEndpointDelegateBuilder.Build<TRequest, TResponse>(successfulStatusCode))                
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
                    OverrideParameterDefinitions = additionalParameterDefinitions ?? new Dictionary<string, OpenApiParameter>()
                });
            configureEndpoint?.Invoke(builder);
        }

        private string AddLeadingSlashIfNotPresent(string path)
        {
            return !path.StartsWith("/") ? $"/{path}" : path;
        }
    }
}
