using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints.Sample
{
    public interface IEndpointContextAccessor
    {
        HttpContext CurrentContext { get; set; }
    }
}