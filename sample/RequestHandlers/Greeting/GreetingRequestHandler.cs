using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AJP.MediatrEndpoints;

namespace mediatr_test.RequestHandlers
{
    public class GreetingRequestHandler : IRequestHandler<ApiRequestWrapper<GreetingRequest, GreetingResponse>, ApiResponseWrapper<GreetingResponse>>
    {
        private readonly ILogger<GreetingRequestHandler> _logger;

        public GreetingRequestHandler(ILogger<GreetingRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task<ApiResponseWrapper<GreetingResponse>> Handle(ApiRequestWrapper<GreetingRequest, GreetingResponse> request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GreetingRequestHandler request received");
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