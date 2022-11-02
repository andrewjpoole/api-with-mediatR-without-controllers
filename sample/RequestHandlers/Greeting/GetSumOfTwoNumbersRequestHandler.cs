using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting;

public class GetSumOfTwoNumbersRequestHandler : IRequestHandler<GetSumOfTwoNumbersRequest, GreetingResponse>
{
    private readonly ILogger<GetSumOfTwoNumbersRequestHandler> _logger;

    public GetSumOfTwoNumbersRequestHandler(ILogger<GetSumOfTwoNumbersRequestHandler> logger)
    {
        _logger = logger;
    }

    public Task<GreetingResponse> Handle(GetSumOfTwoNumbersRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetSumOfTwoNumbersRequestHandler request received");

        var response = new GreetingResponse
        {
            Message = $"Sum of {request.A} and {request.B} is {request.A + request.B}! @{DateTime.Now}"
        };

        return Task.FromResult(response);
    }
}