using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestHandlerWithResponseCode : IRequestHandler<TestRequestWithResponseCode, TestResponseWithResponseCode>
    {
        public Task<TestResponseWithResponseCode> Handle(TestRequestWithResponseCode request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponseWithResponseCode
            {
                StatusCode = 202,
                Message = $"{request.Prop1}"
            });
        }
    }
}