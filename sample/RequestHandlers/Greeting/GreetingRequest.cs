using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Greeting
{
    public class GreetingRequest : IRequest<GreetingResponse>
    {
        public string To { get; set; }        
    }   
}