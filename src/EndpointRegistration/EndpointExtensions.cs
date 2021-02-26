using Microsoft.AspNetCore.Routing;

namespace AJP.MediatrEndpoints
{
    public static class EndpointExtensions
    {
        public static EndpointGroupMapper MapGroupOfEndpointsForAPath(this IEndpointRouteBuilder endpoints, string path, string name, string description = "")
        {
            return new EndpointGroupMapper(endpoints, path, name, description);
        }
    }
}
