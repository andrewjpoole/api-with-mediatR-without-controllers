using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace mediatr_test
{
    public class ApiRequestWrapper<TRequest>
    {
        public IHeaderDictionary Headers {get; private set;}
        public string QueryString { get; private set; }
        public IDictionary<string, object> RouteValues {get; private set;}
        public TRequest Data { get; set; }
    }
}
