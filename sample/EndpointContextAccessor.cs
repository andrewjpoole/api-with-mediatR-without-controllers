using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints.Sample
{
    public class EndpointContextAccessor : IEndpointContextAccessor
    {
        public HttpContext CurrentContext { get; set; } // register as scoped/one per request
    }
}