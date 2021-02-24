using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using AJP.MediatrEndpoints;

namespace mediatr_test.RequestHandlers
{
    public class GreetingGetRequestHandler : IRequestHandler<ApiRequestWrapper<GreetingGetRequest, GreetingGetResponse>, ApiResponseWrapper<GreetingGetResponse>>
    {
        private readonly ILogger<GreetingGetRequestHandler> _logger;

        public GreetingGetRequestHandler(ILogger<GreetingGetRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task<ApiResponseWrapper<GreetingGetResponse>> Handle(ApiRequestWrapper<GreetingGetRequest, GreetingGetResponse> request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GreetingRequestHandler request received");
            var response = new ApiResponseWrapper<GreetingGetResponse>
            {
                StatusCode = 202,
                Data = new GreetingGetResponse
                {
                    Message = $"Greetings {request.Data.To}! @{DateTime.Now}"
                }
            };
            response.Headers.Add(Constants.HeaderKeys_ProcessedAt, DateTime.Now.ToString());
            
            return Task.FromResult(response);
        }
    }
}