using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints.Sample
{
    public class HttpContextAccessor : IMediatrEndpointsContextAccessor
    {
        public HttpContext CurrentContext { get; set; } // register as scoped/one per request
    }
}