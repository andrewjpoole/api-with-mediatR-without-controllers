using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Tests.TestAppImpl
{
    public class TestApp
    {
        private IHost _host;
        
        public TestApp(string uriString)
        {
            _host = CreateHostBuilder(uriString).Build();
            _host.RunAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string uriString) =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(uriString);
                });

        public void StopHost()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }
    }
}