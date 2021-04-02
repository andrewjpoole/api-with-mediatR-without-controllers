using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Tests.TestAppImpl
{
    public class TestApp
    {
        private IHost _host;
        
        public TestApp(string uriString, Func<IServiceCollection, IServiceCollection> servicesTweaker = null)
        {
            _host = CreateHostBuilder(uriString, servicesTweaker).Build();
            _host.RunAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string uriString, Func<IServiceCollection, IServiceCollection> servicesTweaker) =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureServices(services => servicesTweaker?.Invoke(services));
                    webBuilder.UseUrls(uriString);
                });

        public void StopHost()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }
    }
}