using Microsoft.AspNetCore.Routing;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public static class MediatrEndpointExtensions
    {
        public static MediatrEndpointGroupMapper MapGroupOfEndpointsForAPath(this IEndpointRouteBuilder endpoints, string path, string name, string description = "") => 
            new MediatrEndpointGroupMapper(endpoints, path, name, description);
    }
}
