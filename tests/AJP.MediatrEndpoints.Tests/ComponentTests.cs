using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Tests.TestAppImpl;
using AJP.MediatrEndpoints.Tests.TestRequestHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AJP.MediatrEndpoints.Tests
{
    public class ComponentTests : IDisposable
    {
        private MediatorEndpointsTestAppFactory _testAppFactory;
        private HttpClient _client;
        private JsonSerializerOptions _jsonSerializerOptions;
        private TestRequestProcessors _mockRequestRequestProcessors = new TestRequestProcessors();
        
        public ComponentTests()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _testAppFactory = new MediatorEndpointsTestAppFactory(_mockRequestRequestProcessors);
            _client = _testAppFactory.CreateClient();
        }

        private async Task<TestResponse> AssertSuccessAndReturnResponseObject(HttpResponseMessage response)
        {
            Assert.NotNull(response);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var testResponse = await JsonSerializer.DeserializeAsync<TestResponse>(responseBody, _jsonSerializerOptions);

            testResponse.Should().NotBeNull();
            
            return testResponse;
        }

        [Fact]
        public async Task a_GET_request_with_route_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);
            
            testResponse.Message.Should().StartWith("aaaa");
        }
        
        [Fact]
        public async Task a_GET_request_with_querystring_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa?prop2=bbbb");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);

            testResponse.Message.Should().StartWith("aaaa bbbb");
        }
        
        [Fact]
        public async Task a_GET_request_with_header_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            _client.DefaultRequestHeaders.Add("Prop3", "cccc");
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);

            testResponse.Message.Should().StartWith("aaaa  cccc");
        }
        
        [Fact]
        public async Task a_GET_request_with_body_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            var body = new 
            {
                Prop4 = "dddd"
            };
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/1/aaaa"),
                Content = JsonContent.Create(body, null, _jsonSerializerOptions)
            };
            var httpResponseMessage = await _client.SendAsync(request);

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);

            testResponse.Message.Should().StartWith("aaaa   dddd");
        }

        [Fact]
        public async Task a_GET_request_without_required_route_parameter_should_return_400badrequest()
        {
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/2");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        public async Task a_request_should_call_all_processors_once_in_order()
        {
            DateTime postRequestProcessTime = default;
            DateTime preRequestProcessTime = default;
            
            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => preRequestProcessTime = DateTime.Now);
            _mockRequestRequestProcessors.MockPostProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<TimeSpan>(), It.IsAny<ILogger>())).Callback(() => postRequestProcessTime = DateTime.Now);
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            _mockRequestRequestProcessors.MockPreProcessor.Verify(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>()), Times.Once);
            _mockRequestRequestProcessors.MockPostProcessor.Verify(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<TimeSpan>(), It.IsAny<ILogger>()), Times.Once);
            _mockRequestRequestProcessors.MockErrorProcessor.Verify(x => x.Invoke(It.IsAny<Exception>(), It.IsAny<HttpContext>(), It.IsAny<ILogger>()), Times.Never);

            preRequestProcessTime.Should().BeBefore(postRequestProcessTime);
        }
        
        [Fact]
        public async Task a_request_should_call_error_processor_if_an_exception_is_thrown()
        {
            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new Exception("test exception"));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            _mockRequestRequestProcessors.MockErrorProcessor.Verify(x => x.Invoke(It.IsAny<Exception>(), It.IsAny<HttpContext>(), It.IsAny<ILogger>()), Times.Once);
        }
        
        public void Dispose()
        {
            _testAppFactory.Dispose();
        }
    }
}