using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Exceptions;
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
            _testAppFactory = new MediatorEndpointsTestAppFactory();
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
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);
            
            testResponse.Message.Should().StartWith("aaaa");
        }
        
        [Fact]
        public async Task a_GET_request_with_querystring_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa?prop2=bbbb");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);

            testResponse.Message.Should().StartWith("aaaa bbbb");
        }
        
        [Fact]
        public async Task a_GET_request_with_header_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _client.DefaultRequestHeaders.Add("Prop3", "cccc");
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            var testResponse = await AssertSuccessAndReturnResponseObject(httpResponseMessage);

            testResponse.Message.Should().StartWith("aaaa  cccc");
        }
        
        [Fact]
        public async Task a_GET_request_with_body_parameter_should_return_200Ok_and_pass_the_param_through()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

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
        public async Task a_valid_POST_request_should_return_200Ok()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/3/aaaa"),
            };
            var httpResponseMessage = await _client.SendAsync(request);
            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Fact]
        public async Task a_valid_PUT_request_should_return_200Ok()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/4/"),
            };
            var httpResponseMessage = await _client.SendAsync(request);
            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Fact]
        public async Task a_valid_DELETE_request_should_return_200Ok()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/5/"),
            };
            var httpResponseMessage = await _client.SendAsync(request);
            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task a_GET_request_with_auth_required_and_no_api_key_supplied_should_return_401unauthorized()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var httpResponseMessage = await _client.GetAsync("/api/v1/test/7");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task a_GET_request_with_auth_required_and_valid_apikey_passed_should_return_ok()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/7")
            };
            request.Headers.Add("X-API-KEY", "testKey123");
            var httpResponseMessage = await _client.SendAsync(request);

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task a_GET_request_with_invalid_body_json_should_return_400BadRequest()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1/test/2/"),
                Content = new StringContent("{")
            };
            var httpResponseMessage = await _client.SendAsync(request);
            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("Bad request, body is not valid json");
        }
        
        [Fact]
        public async Task a_request_should_return_500internalServerError_if_mediator_not_available()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors, false);
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/2");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
        
        [Fact]
        public async Task a_GET_request_without_required_route_parameter_should_return_400badrequest()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var httpResponseMessage = await _client.GetAsync("/api/v1/test/2");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        public async Task if_StatusCode_property_on_TResponse_is_set_it_should_be_used_as_Response_statusCode()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var httpResponseMessage = await _client.GetAsync("/api/v1/test/2?Prop1=8764");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }
        
        [Fact]
        public async Task a_request_should_call_all_processors_once_in_order()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

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
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new Exception("test exception"));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            _mockRequestRequestProcessors.MockErrorProcessor.Verify(x => x.Invoke(It.IsAny<Exception>(), It.IsAny<HttpContext>(), It.IsAny<ILogger>()), Times.Once);
        }
        
        [Fact]
        public async Task a_request_should_return_if_response_exception_is_thrown()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new CustomHttpResponseException("test exception", false, false, 503));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            httpResponseMessage.StatusCode.Should().Be(503);
        }
        
        [Fact]
        public async Task a_request_should_return_if_response_exception_is_thrown_calling_postprocess()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new CustomHttpResponseException("test exception", true, false, 503));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            httpResponseMessage.StatusCode.Should().Be(503);
            
            _mockRequestRequestProcessors.MockPostProcessor.Verify(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<TimeSpan>(), It.IsAny<ILogger>()), Times.Once);
        }
        
        [Fact]
        public async Task a_request_should_return_if_response_exception_is_thrown_calling_error_process()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new CustomHttpResponseException("test exception", false, true, 503));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            httpResponseMessage.StatusCode.Should().Be(503);
            
            _mockRequestRequestProcessors.MockErrorProcessor.Verify(x => x.Invoke(It.IsAny<Exception>(), It.IsAny<HttpContext>(), It.IsAny<ILogger>()), Times.Once);
        }
        
        [Fact]
        public async Task a_request_should_return_with_no_content_if_statuscode_is_204()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            var httpResponseMessage = await _client.GetAsync("/api/v1/test/6");

            httpResponseMessage.StatusCode.Should().Be(204);
            
            httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult().Should().Be("");
            
        }
        
        [Fact]
        public async Task a_request_should_return_if_notfound_response_exception_is_thrown()
        {
            _testAppFactory.StartTestApp(_mockRequestRequestProcessors);

            _mockRequestRequestProcessors.MockPreProcessor.Setup(x => x.Invoke(It.IsAny<HttpContext>(), It.IsAny<ILogger>())).Callback(() => throw new NotFoundHttpException("test exception","resource not found!"));
            
            var httpResponseMessage = await _client.GetAsync("/api/v1/test/1/aaaa");

            httpResponseMessage.StatusCode.Should().Be(404);

            httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult().Should().Be("resource not found!");
        }
        
        public void Dispose()
        {
            _testAppFactory.Dispose();
        }
    }
}