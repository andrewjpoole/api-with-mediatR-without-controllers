using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace mediatr_test
{
    public class TestRequest : IRequest<ApiResponseWrapper<TestReply>>
    {
        public string To { get; set; }
    }

    public class TestReply
    {
        public string From { get; set; }
    }

    public class TestRequestHandler : IRequestHandler<TestRequest, ApiResponseWrapper<TestReply>> 
    {    
        public Task<ApiResponseWrapper<TestReply>> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ApiResponseWrapper<TestReply> 
            {
                StatusCode = StatusCodes.Status202Accepted,
                Data = new TestReply
                {
                    From = $"response to TestRequest to {request.To} @{DateTime.Now}"
                }
            });
        }
    }
}