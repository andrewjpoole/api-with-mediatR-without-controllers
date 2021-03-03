using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using mediatr_test.RequestHandlers;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.SwaggerSupport;
using Microsoft.OpenApi.Models;
using System;
using mediatr_test.StatisticsGatherer;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using AJP.MediatrEndpoints.EndpointRegistration;
using mediatr_test.RequestHandlers.Accounts;
using mediatr_test.RequestHandlers.Greeting;
using mediatr_test.Services;
using Microsoft.Extensions.FileProviders;

namespace mediatr_test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Api with mediatr without controllers test app",
                    Version = "v1",
                    Description = "test app showing how to use Mediatr RequestHandlers wired up to Endpoints, with Swagger documentation",
                    Contact = new OpenApiContact
                    {
                        Name = "Andrew Poole",
                        Email = string.Empty,
                        Url = new Uri("https://forkinthecode.net/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT license",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
                
                c.DocumentFilter<AddEndpointsDocumentFilter>();
            });
            
            services.AddLogging();
            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IMediatrEndpointsProcessors, RequestProcessors>();
            services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
            services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
            services.AddHostedService(sp => (StatisticsQueuedHostedService)sp.GetService<IStatisticsQueuedHostedService>());
            services.AddSingleton<IAccountRepository, AccountRepository>();
        }
                
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "wwwroot")),
                RequestPath = "/static"
            });
            
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.InjectStylesheet("../static/swagger.css");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
                    .WithGet<GetAccountsRequest, IEnumerable<AccountDetails>>("/")
                    .WithGet<GetAccountByIdRequest, AccountDetails>("/{Id}") // route parameter name must match property on TRequest, including case!! otherwise swagger breaks
                    .WithPost<CreateAccountRequest, AccountDetails>("/")
                    .WithDelete<DeleteAccountByIdRequest, AccountDetails>("/{Id}")
                    .WithPut<UpdateAccountStatusRequest, AccountDetails>("/{Id}");
               
                endpoints.MapGroupOfEndpointsForAPath("/api/v1/greeting", "Greetings", "description of the greetings path")
                    .WithPost<GreetingRequest, GreetingResponse>("/");

                endpoints.MapGet("/Stats", async context =>
                {
                    var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                    await context.Response.WriteAsJsonAsync(statsGatherer?.GetStats());
                });

                endpoints.MapGet("/Endpoints", async context =>
                {
                    var sb = new StringBuilder();
                    var endpointDataSource = context.RequestServices.GetService<EndpointDataSource>();
                    foreach (var endpoint in endpointDataSource.Endpoints)
                        sb.AppendLine($"{endpoint.DisplayName} {endpoint.RequestDelegate}");
                    
                    await context.Response.WriteAsync(sb.ToString());
                });
            });
        }
    }
}
