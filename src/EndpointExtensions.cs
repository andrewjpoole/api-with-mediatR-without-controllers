using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace mediatr_test
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapGetToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {           
            endpoints.MapGet(pattern, MediarREndpointDelagteBuilder.Build<TRequest, TResponse>());
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPostToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapPost(pattern, MediarREndpointDelagteBuilder.Build<TRequest, TResponse>());
            return endpoints;
        }

        public static IEndpointRouteBuilder MapPutToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapPut(pattern, MediarREndpointDelagteBuilder.Build<TRequest, TResponse>());
            return endpoints;
        }

        public static IEndpointRouteBuilder MapDeleteToRequestHandler<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern)
        {
            endpoints.MapDelete(pattern, MediarREndpointDelagteBuilder.Build<TRequest, TResponse>());
            return endpoints;
        }
    }
}
