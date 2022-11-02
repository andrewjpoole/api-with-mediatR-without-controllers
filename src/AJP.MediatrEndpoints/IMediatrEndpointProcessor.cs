using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints
{
    public interface IMediatrEndpointsProcessors
    {
        Action<HttpContext, ILogger> PreProcess {get; set;}
        Action<HttpContext, TimeSpan, ILogger> PostProcess {get; set;}
        Action<Exception, HttpContext, ILogger> ErrorProcess {get; set;}
    }
}
