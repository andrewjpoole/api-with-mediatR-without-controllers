using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Json;

namespace mediatr_test
{
    public static class MediarREndpointDelagteBuilder
    {
        public static RequestDelegate Build<TRequest, TResponse>()
        {
            return async context => {
                var logger = context.RequestServices.GetService<ILogger<TRequest>>();
                var mediator = context.RequestServices.GetService<IMediator>();
                                
                try
                {
                    logger.LogInformation($"{context.Request.Method} {context.Request.Path} request received with queryString:{context.Request.QueryString}");
                    
                    var request = await context.Request.ReadFromJsonAsync<TRequest>();
                    var response = await mediator.Send(request) as ApiResponseWrapper<TResponse>; // send in any route values and query string? may have to wrap the request as well?

                    context.Response.StatusCode = response.StatusCode;                    
                    await context.Response.WriteAsJsonAsync<TResponse>(response.Data);
                    
                    logger.LogInformation($"Sending {context.Response.StatusCode} response");
                }
                catch (System.Exception) // todo catch the serialisation exception and return bad request etc
                {
                    
                    throw;
                }

            };
        }
    }
}
