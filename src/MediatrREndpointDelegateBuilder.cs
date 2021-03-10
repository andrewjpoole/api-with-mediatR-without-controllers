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
using AJP.MediatrEndpoints.Exceptions;
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
                
                var stopwatch = new Stopwatch();
                var jsonSerialiserOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                if (mediator is null)
                    throw new ApplicationException("Could not resolve IMediator from DI container, ensure services.AddMediatR(typeof(Startup)); or similar is configured in Startup->ConfigureServices().");
                
                try
                {
                    requestProcessors?.PreProcess(context, logger);
                    
                    stopwatch.Start();
                    
                    // Start by deserialising the body
                    using var streamReader = new StreamReader(context.Request.BodyReader.AsStream());
                    var bodyJson = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(bodyJson))
                        bodyJson = "{}";
                    
                    var requestObject = JsonDocument.Parse(bodyJson).RootElement;

                    var queryStringValues = SplitQueryString(context.Request.QueryString);
                    
                    // Loop through the public props of the TRequest and try to populate them from body, then route, querystring and headers
                    var propsOnTRequest = typeof(TRequest).GetPublicInstanceProperties();
                    foreach (var propOnTRequest in propsOnTRequest)
                    {
                        var propNameLowerCase = propOnTRequest.Name.ToLower();
                        var propNameCamelCase = JsonNamingPolicy.CamelCase.ConvertName(propOnTRequest.Name);
                        
                        // Check if we already have a property with a value from the body
                        if (requestObject.TryGetProperty(propOnTRequest.Name, out _) 
                            || requestObject.TryGetProperty(propNameLowerCase, out _)
                            || requestObject.TryGetProperty(propNameCamelCase, out _))
                        {
                            continue;
                        }

                        // Check if there is a matching value from the route
                        if (context.Request.RouteValues.Keys.Any(x => x.ToLower() == propNameLowerCase))
                        {
                            requestObject = requestObject.AddProperty(propNameLowerCase,
                                context.Request.RouteValues.First(x => x.Key.ToLower() == propNameLowerCase).Value);
                            continue;
                        }
                        
                        // Check the query string
                        if (queryStringValues.Keys.Any(x => x.ToLower() == propNameLowerCase))
                        {
                            requestObject = requestObject.AddProperty(propNameLowerCase,
                                queryStringValues.First(x => x.Key.ToLower() == propNameLowerCase).Value);
                            continue;
                        }

                        // Check if there is a matching value from the request headers
                        if (context.Request.Headers.Keys.Any(x => x.ToLower() == propNameLowerCase))
                        {
                            requestObject = requestObject.AddProperty(propNameLowerCase,
                                context.Request.Headers.First(x => x.Key.ToLower() == propNameLowerCase).Value.FirstOrDefault());
                            continue;
                        }

                        // Property not found so far, check if its optional
                        if (propOnTRequest.GetCustomAttributes(typeof(OptionalPropertyAttribute)).Any())
                        {
                            continue;
                        }

                        // Cant find a match for required property
                        throw new BadHttpRequestException($"Missing Property {propOnTRequest.Name}");
                    }

                    var mediatrRequest = requestObject.ConvertToObject<TRequest>(jsonSerialiserOptions);
                    var mediatrResponse = (TResponse) await mediator.Send(mediatrRequest);

                    if (mediatrResponse.HasStatusCodeProperty(out var statusCode))
                        context.Response.StatusCode = statusCode;
                    
                    stopwatch.Stop();
                    requestProcessors?.PostProcess(context, stopwatch.Elapsed, logger);
                    await context.Response.WriteAsJsonAsync(mediatrResponse, jsonSerialiserOptions);
                }
                catch (JsonException ex)
                {
                    if(stopwatch.IsRunning)
                            stopwatch.Stop();
                    
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync("Bad request, body is not valid json");
                }
                catch (BadHttpRequestException ex)
                {
                    if(stopwatch.IsRunning)
                        stopwatch.Stop();

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync(ex.Message);
                }
                catch (NotFoundHttpException ex)
                {
                    if(stopwatch.IsRunning)
                        stopwatch.Stop();
                    
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync(ex.ResponseBody);
                }
                catch (CustomHttpResponseException ex)
                {
                    if(stopwatch.IsRunning)
                        stopwatch.Stop();

                    context.Response.StatusCode = ex.ResponseStatusCode;
                    
                    if(ex.TriggerPostProcessor)
                        requestProcessors?.PostProcess(context, stopwatch.Elapsed, logger);
                    
                    if(ex.TriggerErrorProcessor)
                        requestProcessors?.ErrorProcess(ex, context, logger);
                    
                    await context.Response.WriteAsync(ex.ResponseBody);
                }
                catch (Exception ex)
                {
                    if(stopwatch.IsRunning)
                        stopwatch.Stop();

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    requestProcessors?.ErrorProcess(ex, context, logger);
                    await context.Response.WriteAsync("Something has gone wrong.");
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

        private static IEnumerable<PropertyInfo> GetPublicInstanceProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        }
        
        private static bool HasStatusCodeProperty<TResponse>(this TResponse response, out int statusCode)
        {
            statusCode = -1;
            var props = typeof(TResponse).GetPublicInstanceProperties();
            var statusCodeProp = props.FirstOrDefault(x => x.Name == "StatusCode");
            if (statusCodeProp == null)
                return false;

            statusCode = (int)statusCodeProp.GetValue(response);
            return true;
        }
    }
}
