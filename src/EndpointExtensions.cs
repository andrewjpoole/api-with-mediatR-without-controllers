using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapGetToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string displayName, string description = "")
        {
            endpoints.MapGet(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Get, displayName, description));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPostToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string displayName, string description = "")
        {
            endpoints.MapPost(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Post, displayName, description));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPutToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string displayName, string description = "")
        {
            endpoints.MapPut(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Put, displayName, description));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapDeleteToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string displayName, string description = "")
        {
            endpoints.MapDelete(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Delete, displayName, description));
            return endpoints;
        }
    }
}
