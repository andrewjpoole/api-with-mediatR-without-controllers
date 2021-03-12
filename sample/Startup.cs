using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts;
using AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting;
using AJP.MediatrEndpoints.Sample.Services;
using AJP.MediatrEndpoints.Sample.StatisticsGatherer;
using AJP.MediatrEndpoints.SwaggerSupport;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AJP.MediatrEndpoints.Sample
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
            services.AddScoped<IEndpointContextAccessor, EndpointContextAccessor>();
            services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(RequestGenericExceptionHandler<,,>));
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

                // endpoints.MapGet("/api/v1/greeting", async context =>
                // {
                //     var mediatorRequest = await context.Request.ReadFromJsonAsync<GreetingRequest>();
                //     var mediator = context.RequestServices.GetService<IMediator>();
                //     var mediatorResponse = await mediator.Send(mediatorRequest);
                //     await context.Response.WriteAsJsonAsync(mediatorResponse);
                // });
                
                endpoints.MapGroupOfEndpointsForAPath("/api/v1/greeting")
                    .WithPost<GreetingRequest, GreetingResponse>("/");
                
                endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
                    .WithGet<GetAccountsRequest, IEnumerable<AccountDetails>>("/")
                    .WithGet<GetAccountByIdRequest, AccountDetails>("/{Id}")
                    .WithPost<CreateAccountRequest, AccountDetails>("/")
                    .WithDelete<DeleteAccountByIdRequest, AccountDeletedResponse>("/{Id}")
                    .WithPut<UpdateAccountStatusRequest, AccountDetails>("/{Id}");
               
                endpoints.MapGet("/Stats", async context =>
                {
                    var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                    await context.Response.WriteAsJsonAsync(statsGatherer?.GetStats());
                });
            });
        }
    }
}
