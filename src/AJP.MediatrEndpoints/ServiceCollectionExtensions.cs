using System;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace AJP.MediatrEndpoints
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatrEndpoints(this IServiceCollection services, Type assemblyContainsHandlersToRegister)
        {
            services.AddMediatR(assemblyContainsHandlersToRegister);
            services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(RequestGenericExceptionHandler<,,>));

            return services;
        }
    }
}