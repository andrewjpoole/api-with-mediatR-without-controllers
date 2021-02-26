using Microsoft.AspNetCore.Routing;

namespace AJP.MediatrEndpoints.EndpointRegistration
{
    public static class EndpointExtensions
    {
        public static EndpointGroupMapper MapGroupOfEndpointsForAPath(this IEndpointRouteBuilder endpoints, string path, string name, string description = "") => 
            new EndpointGroupMapper(endpoints, path, name, description);
    }
}
