using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AJP.MediatrEndpoints.Tests.TestAppImpl
{
    public class TestRequestProcessors : IMediatrEndpointsProcessors
    {
        public Action<HttpContext, ILogger> PreProcess { get; set; }
        public Action<HttpContext, TimeSpan, ILogger> PostProcess { get; set; }
        public Action<Exception, HttpContext, ILogger> ErrorProcess { get; set; }

        public readonly Mock<Action<HttpContext, ILogger>> MockPreProcessor = new Mock<Action<HttpContext, ILogger>>();

        public readonly Mock<Action<HttpContext, TimeSpan, ILogger>> MockPostProcessor =
            new Mock<Action<HttpContext, TimeSpan, ILogger>>();

        public readonly Mock<Action<Exception, HttpContext, ILogger>> MockErrorProcessor =
            new Mock<Action<Exception, HttpContext, ILogger>>();
        
        public TestRequestProcessors()
        {
            PreProcess = MockPreProcessor.Object;
            PostProcess = MockPostProcessor.Object;
            ErrorProcess = MockErrorProcessor.Object;
        }
    }
}