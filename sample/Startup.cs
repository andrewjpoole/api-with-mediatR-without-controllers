using System;
using System.Collections.Generic;
using System.IO;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts;
using AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting;
using AJP.MediatrEndpoints.Sample.Services;
using AJP.MediatrEndpoints.Sample.StatisticsGatherer;
using AJP.MediatrEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

            services.AddMediatrEndpoints(typeof(Startup));
            services.AddMediatrEndpointsSwagger();
            
            services.AddLogging();
            services.AddSingleton<IMediatrEndpointsProcessors, RequestProcessors>();
            services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
            services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
            services.AddHostedService(sp => (StatisticsQueuedHostedService)sp.GetService<IStatisticsQueuedHostedService>());
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddScoped<IEndpointContextAccessor, EndpointContextAccessor>();
        }
                
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                
                endpoints.MapGroupOfEndpointsForAPath("/api/v1/greeting") // Define swagger operation here rather than using Attributes
                    .WithPost<GreetingRequest, GreetingResponse>("/",string.Empty, StatusCodes.Status200OK, 
                        ParameterDictionaryBuilder
                            .NewDictionary()
                            .AddStringParam(
                                "To", 
                                ParameterDictionaryBuilder.In.Query, 
                                true, 
                                "blah blah"));
                
                endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
                    .WithGet<GetAccountsRequest, IEnumerable<AccountDetails>>("/", "Gets Accounts with various filter options")
                    .WithGet<GetAccountByIdRequest, AccountDetails>("/{Id}", "Get a single account by Id")
                    .WithPost<CreateAccountRequest, CreateAccountResponse>("/", "Create a new account", StatusCodes.Status201Created)
                    .WithDelete<DeleteAccountByIdRequest, AccountDeletedResponse>("/{Id}", "Delete an account by Id", StatusCodes.Status204NoContent)
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
