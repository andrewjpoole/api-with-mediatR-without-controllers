using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting;

public class GetSumOfTwoNumbersRequest : IRequest<GreetingResponse>
{
    public int A { get; set; }
    public int B { get; set; }
}