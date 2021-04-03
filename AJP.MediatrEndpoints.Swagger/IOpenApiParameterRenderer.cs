using System.Collections.Generic;
using System.Text.Json;
using AJP.MediatrEndpoints.EndpointRegistration;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.Swagger
{
    public interface IOpenApiParameterRenderer
    {
        (IEnumerable<OpenApiParameter> Parameters, JsonElement bodyExampleObject) Render(EndpointMetadataDecoratorAttribute swaggerDecorator);
    }
}