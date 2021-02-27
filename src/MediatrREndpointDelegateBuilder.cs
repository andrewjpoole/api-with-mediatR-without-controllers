using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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
                    
                    // Start by deserialising the body
                    using var streamReader = new StreamReader(context.Request.BodyReader.AsStream());
                    var bodyJson = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(bodyJson))
                        bodyJson = "{}";
                    var requestObject = JsonDocument.Parse(bodyJson).RootElement;
                    var requestObjectProps = requestObject.GetProperties().ToList();
                    
                    // Loop through the public props of the TRequest and try to populate them from body, then route, then headers
                    var requiredProps = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                    foreach (var requiredProp in requiredProps)
                    {
                        // Check if we already have a property with a value from the body
                        if (requestObjectProps.Any(x => x.Name == requiredProp.Name))
                        {
                            var potentialMatch = requestObjectProps.Single(x => x.Name == requiredProp.Name);
                            if (potentialMatch.Value != null)
                            {
                                if (potentialMatch.Value.GetType().IsAssignableFrom(requiredProp.PropertyType))
                                {
                                    continue; // looks good, move on
                                }
                            }
                        }

                        // Check if there is a matching value from the route
                        if (context.Request.RouteValues.ContainsKey(requiredProp.Name))
                        {
                            var valueCastToCorrectType = context.Request.RouteValues[requiredProp.Name] as string;
                            
                            requestObject = requestObject.AddProperty(requiredProp.Name,
                                valueCastToCorrectType);
                            continue;
                        }

                        // Check if there is a matching value from the request headers
                        if (context.Request.Headers.ContainsKey(requiredProp.Name))
                        {
                            requestObject = requestObject.AddProperty(requiredProp.Name,
                                context.Request.Headers[requiredProp.Name].FirstOrDefault());
                            continue;
                        }
                        
                        // check if it is optional??
                        
                        // Cant find a match
                        throw new BadHttpRequestException($"Missing Property {requiredProp.Name}");
                    }

                    //var details = new ApiRequestDetails(context.Request.Headers, context.Request.QueryString, context.Request.RouteValues);
                    //var data = await context.Request.ReadFromJsonAsync<TRequest>();
                    
                    // var request = new ApiRequestWrapper<TRequest, TResponse>
                    // {
                    //     Details = details,
                    //     Data = data
                    // };
                    //var mediatrResponseWrapper = await mediator.Send(request) as ApiResponseWrapper<TResponse>;

                    var mediatrRequest = requestObject.ToObject<TRequest>();
                    var mediatrResponse = (TResponse) await mediator.Send(mediatrRequest);
                    //var mediatrResponseWrapper = await mediator.Send(request) as ApiResponseWrapper<TResponse>;

                    // foreach (var header in mediatrResponseWrapper.Headers)
                    // {
                    //     context.Response.Headers.Add(header.Key, header.Value);
                    // }
                    //
                    // context.Response.StatusCode = mediatrResponseWrapper.StatusCode;

                    stopwatch.Stop();
                    requestProcessors?.PostProcess(context, stopwatch.Elapsed, logger);
                    //await context.Response.WriteAsJsonAsync<TResponse>(mediatrResponseWrapper.Data);
                    await context.Response.WriteAsJsonAsync<TResponse>(mediatrResponse);
                }
                catch (JsonException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync("Bad request, body was not valid json");
                }
                catch (BadHttpRequestException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync(ex.Message);
                }
                catch (Exception ex)
                {
                    requestProcessors?.ErrorProcess(ex, context, logger);
                }
            };
        }
    }
    
    public static partial class JsonExtensions
    {
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);
            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        public static T ToObject<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToObject<T>(options);
        }       
    }
}
