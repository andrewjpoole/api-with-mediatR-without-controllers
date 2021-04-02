using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestHandler : IRequestHandler<TestRequestWithRouteParam, TestResponse>
    {
        public Task<TestResponse> Handle(TestRequestWithRouteParam requestWithRouteParam, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponse
            {
                Message = $"{requestWithRouteParam.Prop1} {requestWithRouteParam.Prop2} {requestWithRouteParam.Prop3} {requestWithRouteParam.Prop4}"
            });
        }
    }
}