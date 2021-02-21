using Microsoft.AspNetCore.Http;

namespace mediatr_test
{
    public class ApiResponseWrapper<TResponse>
    {
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public IHeaderDictionary Headers {get; set;} = new HeaderDictionary();
        public TResponse Data { get; set; }
    }
}
