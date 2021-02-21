using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using System;

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
                    string correlationId;
                    if(context.Request.Headers.ContainsKey(Constants.HeaderKeys_CorrelationId))
                    {
                        correlationId = context.Request.Headers[Constants.HeaderKeys_CorrelationId].ToString();
                    }
                    else
                    {
                        correlationId = Guid.NewGuid().ToString();
                        context.Request.Headers.Add(Constants.HeaderKeys_CorrelationId, correlationId);
                    }

                    logger.LogInformation($"{context.Request.Method} {context.Request.Path} request received with queryString:{context.Request.QueryString} and CorrelationId:{correlationId}");
                    
                    var data = await context.Request.ReadFromJsonAsync<TRequest>();                    
                    var details = new ApiRequestDetails(context.Request.Headers, context.Request.QueryString, context.Request.RouteValues);
                    
                    var request = new ApiRequestWrapper<TRequest, TResponse>
                    {
                        Details = details,
                        Data = data
                    };
                    var response = await mediator.Send(request) as ApiResponseWrapper<TResponse>;

                    foreach(var header in response.Headers)
                    {
                        context.Response.Headers.Add(header.Key, header.Value);
                    }

                    context.Response.Headers.Add(Constants.HeaderKeys_CorrelationId, correlationId);
                    context.Response.Headers.Add(Constants.HeaderKeys_Node, Environment.MachineName);

                    context.Response.StatusCode = response.StatusCode;                    
                    await context.Response.WriteAsJsonAsync<TResponse>(response.Data);
                    
                    logger.LogInformation($"Sending {context.Response.StatusCode} response");
                }
                catch (System.Exception) // todo catch any serialisation exception and return bad request etc
                {
                    
                    throw;
                }

            };
        }
    }
}
