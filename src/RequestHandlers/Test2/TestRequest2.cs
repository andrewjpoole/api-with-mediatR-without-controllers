using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace mediatr_test.RequestHandlers.Test
{
    public class TestRequest2 : IRequest<ApiResponseWrapper<TestReply>>
    {
        public string To { get; set; }
    }

    public class TestReply2
    {
        public string From { get; set; }
    }

    public class TestRequestHandler : IRequestHandler<ApiRequestWrapper<TestRequest2>, ApiResponseWrapper<TestReply2>>
    {
        public Task<ApiResponseWrapper<TestReply2>> Handle(ApiRequestWrapper<TestRequest2> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ApiResponseWrapper<TestReply2>
            {
                StatusCode = StatusCodes.Status202Accepted,
                Data = new TestReply2
                {
                    From = $"response to TestRequest to {request.Data.To} @{DateTime.Now}"
                }
            });
        }
    }

    public class ApiRequestHandler<TRequest, TResponse> : IRequestHandler<ApiRequestWrapper<TRequest>, ApiResponseWrapper<TResponse>>
    {
        public Task<ApiResponseWrapper<TResponse>> Handle(ApiResponseWrapper<TRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}