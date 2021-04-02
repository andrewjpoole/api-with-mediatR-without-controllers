using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestWithResponseCode: IRequest<TestResponseWithResponseCode>
    {
        public string Prop1 { get; init; }
    }
}