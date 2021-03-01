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
using AJP.MediatrEndpoints.EndpointRegistration;
using mediatr_test.RequestHandlers.Accounts;
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

                endpoints.MapGet("/api/v1/accounts", 
                    MediatrREndpointDelegateBuilder.Build<GetAccountsRequest, IEnumerable<AccountDetails>>())
                    .WithMetadata(new SwaggerEndpointDecoraterAttribute
                    {
                        //EndpointGroupPath = _path,
                        //EndpointGroupName = _name,
                        //EndpointGroupDescription = _description,
                        Pattern = "/api/v1/accounts",
                        OperationType = OperationType.Get,
                        RequestType = typeof(GetAccountsRequest),
                        ResponseType = typeof(IEnumerable<AccountDetails>),
                        //SwaggerOperationDescription = swaggerOperationDescription,
                        //AdditionalParameterDefinitions = additionalParameterDefinitions
                    });
                
                endpoints.MapGet("/api/v1/accounts/{id}", MediatrREndpointDelegateBuilder.Build<GetAccountByIdRequest, AccountDetails>());
                
                //var idParameter = new OpenApiParameter
                //{
                //    In = ParameterLocation.Path,
                //    Name = "id",
                //    Schema = new OpenApiSchema { Type = "int" }
                //};

                var additionalParamsWithRouteId = AdditionalParameter.NewDictionary()
                    .AddStringParam("id", AdditionalParameter.In.Route);

                endpoints.MapGroupOfEndpointsForAPath("/api/v1/greeting", "Greetings", "description of the greetings path")
                    .WithPost<GreetingGetRequest, GreetingGetResponse>("/", "description of the get all operation")
                    .WithPost<GreetingGetRequest, GreetingGetResponse>("/{id}", "description of the get by id operation", 
                        additionalParamsWithRouteId
                        .AddBoolParam("IncludeOldGreetings", AdditionalParameter.In.Query)
                        .AddEnumParam("Animal", typeof(Animals), AdditionalParameter.In.Query, true))
                    .WithDelete<GreetingGetRequest, GreetingGetResponse>("/{id}", "used to delete a greeting", additionalParamsWithRouteId);

                endpoints.MapGet("/Stats", async context =>
                {
                    var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                    await context.Response.WriteAsJsonAsync(statsGatherer.GetStats());
                });
            });
        }
    }

    public enum Animals 
    {
        bear,
        dog,
        fox,
        pig
    }
}
