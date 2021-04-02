using System;
using System.Net.Http;
using AJP.MediatrEndpoints.Tests.TestAppImpl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace AJP.MediatrEndpoints.Tests
{
    public class MediatorEndpointsTestAppFactory : IDisposable
    {
        private string _baseUrl = "http://localhost:5003";
        private TestApp _testApp;
        
        public void StartTestApp(IMediatrEndpointsProcessors requestProcessors, bool addMediator = true)
        {
            Startup.TestRequestProcessors = requestProcessors;
            Startup.AddMediatorService = addMediator;
            _testApp = new TestApp(_baseUrl);
        }
        
        public HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            return client;
        }

        public void Dispose()
        {
            _testApp.StopHost();
        }
    }
}