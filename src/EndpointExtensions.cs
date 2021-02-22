using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapGetToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapGet(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Get));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPostToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapPost(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Post));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPutToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapPut(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Put));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapDeleteToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapDelete(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerDecoraterAttribute(typeof(TRequest), typeof(TResponse), OperationType.Delete));
            return endpoints;
        }
    }
}
