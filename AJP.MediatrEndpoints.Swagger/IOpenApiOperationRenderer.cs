using AJP.MediatrEndpoints.EndpointRegistration;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AJP.MediatrEndpoints.Swagger
{
    public interface IOpenApiOperationRenderer
    {
        OpenApiOperation Render(
            Endpoint endpoint,
            EndpointMetadataDecoratorAttribute swaggerDecorator, 
            OpenApiDocument swaggerDoc,
            DocumentFilterContext context);
    }
}