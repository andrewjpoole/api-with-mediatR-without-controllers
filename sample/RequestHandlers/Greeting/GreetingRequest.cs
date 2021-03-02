using MediatR;

namespace mediatr_test.RequestHandlers.Greeting
{
    public class GreetingRequest : IRequest<GreetingResponse>
    {
        public string To { get; set; }        
    }   
}