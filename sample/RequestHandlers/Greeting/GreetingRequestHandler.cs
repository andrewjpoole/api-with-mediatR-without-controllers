using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting
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
                Message = $"Greetings {request.To} aged {request.Age}! @{DateTime.Now}"
            };
            
            return Task.FromResult(response);
        }
    }
}