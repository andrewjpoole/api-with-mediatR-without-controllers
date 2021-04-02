using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestHandlerWithNoContentResponseCode : IRequestHandler<TestRequestWithNoContentResponseCode, TestResponseWithNoContentResponseCode>
    {
        public Task<TestResponseWithNoContentResponseCode> Handle(TestRequestWithNoContentResponseCode request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponseWithNoContentResponseCode
            {
                StatusCode = 204
            });
        }
    }
}