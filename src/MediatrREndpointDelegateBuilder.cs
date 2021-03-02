using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AJP.MediatrEndpoints.PropertyAttributes;

namespace AJP.MediatrEndpoints
{
    public static class MediatrREndpointDelegateBuilder
    {
        public static RequestDelegate Build<TRequest, TResponse>()
        {
            return async context => {
                var logger = context.RequestServices.GetService<ILogger<TRequest>>();
                var requestProcessors = context.RequestServices.GetService<IMediatrEndpointsProcessors>();
                var mediator = context.RequestServices.GetService<IMediator>();

                if (mediator is null)
                    throw new ApplicationException("Could not resolve IMediator from DI container, ensure services.AddMediatR(typeof(Startup)); or similar is configured in Startup->ConfigureServices().");
                
                try
                {
                    requestProcessors?.PreProcess(context, logger);

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    
                    // Start by deserialising the body
                    using var streamReader = new StreamReader(context.Request.BodyReader.AsStream());
                    var bodyJson = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(bodyJson))
                        bodyJson = "{}";
                    
                    var requestObject = JsonDocument.Parse(bodyJson).RootElement;

                    var queryStringValues = SplitQueryString(context.Request.QueryString);
                    
                    // Loop through the public props of the TRequest and try to populate them from body, then route, then headers
                    var requiredPropsOnTRequest = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
                    foreach (var requiredProp in requiredPropsOnTRequest)
                    {
                        // Check if we already have a property with a value from the body
                        if (requestObject.TryGetProperty(requiredProp.Name, out var matchedProperty))
                        {
                            // RequiredProperty already exists on the body
                            continue;
                        }

                        // Check if there is a matching value from the route
                        if (context.Request.RouteValues.ContainsKey(requiredProp.Name))
                        {
                            requestObject = requestObject.AddProperty(requiredProp.Name,
                                context.Request.RouteValues[requiredProp.Name]);
                            continue;
                        }
                        
                        // Check the query string
                        if (queryStringValues.ContainsKey(requiredProp.Name))
                        {
                            requestObject = requestObject.AddProperty(requiredProp.Name,
                                queryStringValues[requiredProp.Name]);
                            continue;
                        }

                        // Check if there is a matching value from the request headers
                        if (context.Request.Headers.ContainsKey(requiredProp.Name))
                        {
                            requestObject = requestObject.AddProperty(requiredProp.Name,
                                context.Request.Headers[requiredProp.Name].FirstOrDefault());
                            continue;
                        }

                        if (requiredProp.GetCustomAttributes(typeof(OptionalPropertyAttribute)).Any())
                        {
                            // Property is optional, dont worry
                            continue;
                        }

                        // Cant find a match
                        throw new BadHttpRequestException($"Missing Property {requiredProp.Name}");
                    }

                    var mediatrRequest = requestObject.ToObject<TRequest>();
                    var mediatrResponse = (TResponse) await mediator.Send(mediatrRequest);
                    
                    stopwatch.Stop();
                    requestProcessors?.PostProcess(context, stopwatch.Elapsed, logger);
                    await context.Response.WriteAsJsonAsync(mediatrResponse);
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

        private static Dictionary<string, string> SplitQueryString(QueryString queryString)
        {
            var queryStringValues = new Dictionary<string, string>();
            if (!queryString.HasValue)
                return queryStringValues;

            var queryStringSplit = queryString.Value.Replace("?", string.Empty).Split("&");
            foreach (var queryPair in queryStringSplit)
            {
                var pair = queryPair.Split("=");
                queryStringValues.Add(pair[0], pair[1]);
            }

            return queryStringValues;
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
