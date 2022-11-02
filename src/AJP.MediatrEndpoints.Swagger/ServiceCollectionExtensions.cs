using Microsoft.Extensions.DependencyInjection;

namespace AJP.MediatrEndpoints.Swagger
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatrEndpointsSwagger(this IServiceCollection services)
        {
            services.AddSingleton<IOpenApiOperationRenderer, OpenApiOperationRenderer>();
            services.AddSingleton<IOpenApiParameterRenderer, OpenApiParameterRenderer>();

            return services;
        }
    }
}