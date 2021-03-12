using System;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints
{
    public class MediatrEndpointExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public MediatrEndpointExceptionHandlerMiddleware(RequestDelegate next) => this.next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = "";
            switch (exception)
            {
                case NotFoundHttpException ex:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    result = "Not found";
                    break;
                case CustomHttpResponseException ex:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    result = "Not found";
                    break;
                default:
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    result = "Something went wrong";
                    break;
            }

            return context.Response.WriteAsync(result);
        }
    }
}