using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestHandlerPlain : IRequestHandler<TestRequestPlain, TestResponse>
    {
        public Task<TestResponse> Handle(TestRequestPlain request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponse());
        }
    }
}