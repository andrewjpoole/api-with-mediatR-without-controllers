using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints
{
    public interface IMediatrEndpointsContextAccessor
    {
        HttpContext CurrentContext { get; set; }
    }
}