using MediatR;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequestWithNoContentResponseCode: IRequest<TestResponseWithNoContentResponseCode>
    {
    }
}