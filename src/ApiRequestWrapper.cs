using MediatR;

namespace AJP.MediatrEndpoints
{
    public class ApiRequestWrapper<TRequest, TResponse> : IRequest<ApiResponseWrapper<TResponse>>
    {
        public TRequest Data { get; set; }
        public ApiRequestDetails Details { get; set; }
    }
}
