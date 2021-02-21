using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace mediatr_test.RequestHandlers
{
    public class TestRequest2Handler : IRequestHandler<ApiRequestWrapper<GreetingRequest, GreetingResponse>, ApiResponseWrapper<GreetingResponse>>
    {
        private readonly ILogger<TestRequest2Handler> _logger;

        public TestRequest2Handler(ILogger<TestRequest2Handler> logger)
        {
            _logger = logger;
        }

        public Task<ApiResponseWrapper<GreetingResponse>> Handle(ApiRequestWrapper<GreetingRequest, GreetingResponse> request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"TestRequest2 received");
            var response = new ApiResponseWrapper<GreetingResponse>
            {
                StatusCode = StatusCodes.Status202Accepted,
                Data = new GreetingResponse
                {
                    Message = $"Greetings {request.Data.To}! @{DateTime.Now}"
                }
            };
            response.Headers.Add(Constants.HeaderKeys_ProcessedAt, DateTime.Now.ToString());
            
            return Task.FromResult(response);
        }
    }
}