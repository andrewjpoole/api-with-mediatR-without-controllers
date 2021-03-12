using System;
using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusCodeAttribute: Attribute
    {
        public int StatusCode { get; }
        public string StatusCodeName { get; }

        public StatusCodeAttribute(int statusCode, string statusCodeName = "Ok")
        {
            StatusCode = statusCode;
            StatusCodeName = statusCodeName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusCode204NoContentAttribute: StatusCodeAttribute
    {
        public StatusCode204NoContentAttribute() : base(StatusCodes.Status204NoContent, "No Content")
        {
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusCode201CreatedAttribute: StatusCodeAttribute
    {
        public StatusCode201CreatedAttribute() : base(StatusCodes.Status201Created, "Created")
        {
        }
    }
}