using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Tests.TestRequestHandlers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJP.MediatrEndpoints.Tests.TestAppImpl
{
    public class Startup
    {
        public static IMediatrEndpointsProcessors TestRequestProcessors;
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGroupOfEndpointsForAPath("/api/v1/test", "Test", "everything to do with test")
                    .WithGet<TestRequest, TestResponse>("/1/{Prop1}")
                    .WithGet<TestRequest, TestResponse>("/2/");
            });
        }
    }
}