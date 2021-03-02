using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace mediatr_test.RequestHandlers.Greeting
{
    public class GreetingRequestHandler : IRequestHandler<GreetingRequest, GreetingResponse>
    {
        private readonly ILogger<GreetingRequestHandler> _logger;

        public GreetingRequestHandler(ILogger<GreetingRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task<GreetingResponse> Handle(GreetingRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GreetingRequestHandler request received");
            
            var response = new GreetingResponse
            {
                    Message = $"Greetings {request.To}! @{DateTime.Now}"
            };
            
            return Task.FromResult(response);
        }
    }
}