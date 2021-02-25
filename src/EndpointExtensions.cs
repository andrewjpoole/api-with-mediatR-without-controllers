using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapGetToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, SwaggerDocumentationEndpointGroup endpointGroup, string swaggerOperationDescription = "")
        {
            endpoints.MapGet(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerEndpointDecoraterAttribute(pattern, typeof(TRequest), typeof(TResponse), OperationType.Get, endpointGroup, swaggerOperationDescription));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPostToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, SwaggerDocumentationEndpointGroup endpointGroup, string swaggerOperationDescription = "")
        {
            endpoints.MapPost(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerEndpointDecoraterAttribute(pattern, typeof(TRequest), typeof(TResponse), OperationType.Post, endpointGroup, swaggerOperationDescription));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPutToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, SwaggerDocumentationEndpointGroup endpointGroup, string swaggerOperationDescription = "")
        {
            endpoints.MapPut(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerEndpointDecoraterAttribute(pattern, typeof(TRequest), typeof(TResponse), OperationType.Put, endpointGroup, swaggerOperationDescription));
            return endpoints;
        }

        public static IEndpointRouteBuilder MapDeleteToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, SwaggerDocumentationEndpointGroup endpointGroup, string swaggerOperationDescription = "")
        {
            endpoints.MapDelete(pattern, MediatrREndpointDelegateBuilder.Build<TRequest, TResponse>())
                .WithMetadata(new SwaggerEndpointDecoraterAttribute(pattern, typeof(TRequest), typeof(TResponse), OperationType.Delete, endpointGroup, swaggerOperationDescription));
            return endpoints;
        }
    }
}
