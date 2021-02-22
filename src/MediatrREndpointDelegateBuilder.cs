using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace AJP.MediatrEndpoints
{
    public static class MediatrREndpointDelegateBuilder
    {
        public static RequestDelegate Build<TRequest, TResponse>()
        {
            return async context => {
                var logger = context.RequestServices.GetService<ILogger<TRequest>>();
                var mediator = context.RequestServices.GetService<IMediator>();
                var requestProcessors = context.RequestServices.GetService<IMediatrEndpointsProcessors>();
                Stopwatch stopwatch;        
                try
                {
                    requestProcessors?.PreProcess(context, logger);

                    stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var details = new ApiRequestDetails(context.Request.Headers, context.Request.QueryString, context.Request.RouteValues);
                    var data = await context.Request.ReadFromJsonAsync<TRequest>();                    
                    
                    var request = new ApiRequestWrapper<TRequest, TResponse>
                    {
                        Details = details,
                        Data = data
                    };
                    var mediatrResponseWrapper = await mediator.Send(request) as ApiResponseWrapper<TResponse>;

                    foreach(var header in mediatrResponseWrapper.Headers)
                    {
                        context.Response.Headers.Add(header.Key, header.Value);
                    }                    
                    
                    context.Response.StatusCode = mediatrResponseWrapper.StatusCode;

                    stopwatch.Stop();                  
                    requestProcessors?.PostProcess(context, stopwatch.Elapsed, logger);
                    await context.Response.WriteAsJsonAsync<TResponse>(mediatrResponseWrapper.Data);
                }
                catch (Exception ex) // todo catch any serialisation exception and return bad request etc
                {
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    throw;
                }
            };
        }
    }
}
