using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Tests.TestRequestHandlers;
using AspNetCore.Authentication.ApiKey;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Tests.TestAppImpl
{
    public class Startup
    {
        public static IMediatrEndpointsProcessors TestRequestProcessors;
        public static bool AddMediatorService = true;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();

            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
                {
                    options.Realm = "Sample Web API";
                    options.KeyName = "X-API-KEY";
                });

            if (AddMediatorService)
                services.AddMediatR(typeof(Startup));

            TestRequestProcessors ??= new TestRequestProcessors();
            services.AddSingleton(TestRequestProcessors);
        }
                
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

            endpoints.MapGroupOfEndpointsForAPath("api/v1/test", "Test", "everything to do with test")
                .WithGet<TestRequestWithRouteParam, TestResponse>("/1/{Prop1}")
                .WithGet<TestRequestWithResponseCode, TestResponseWithResponseCode>("2/")
                .WithPost<TestRequestWithRouteParam, TestResponse>("/3/{Prop1}")
                .WithPut<TestRequestPlain, TestResponse>("/4/")
                .WithDelete<TestRequestPlain, TestResponse>("/5/")
                .WithGet<TestRequestWithNoContentResponseCode, TestResponseWithNoContentResponseCode>("/6")
                .WithGet<TestRequestPlain, TestResponse>("/7", configureEndpoint: endpoint => endpoint.RequireAuthorization());
                
            });
        }
    }

    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly ILogger<IApiKeyProvider> _logger;

        public ApiKeyProvider(ILogger<IApiKeyProvider> logger)
        {
            _logger = logger;
        }

        public async Task<IApiKey> ProvideAsync(string key)
        {
            try
            {
                return key == "testKey123" ? new ApiKey(key, "test") : null;
            }
            catch (System.Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }

    class ApiKey : IApiKey
    {
        public ApiKey(string key, string owner, List<Claim> claims = null)
        {
            Key = key;
            OwnerName = owner;
            Claims = claims ?? new List<Claim>();
        }

        public string Key { get; }
        public string OwnerName { get; }
        public IReadOnlyCollection<Claim> Claims { get; }
    }
}