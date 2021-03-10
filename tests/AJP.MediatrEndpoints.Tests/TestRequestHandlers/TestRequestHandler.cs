using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
    {
        public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponse
            {
                Message = $"{request.Prop1} {request.Prop2} {request.Prop3} {request.Prop4}"
            });
        }
    }
}