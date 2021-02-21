using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints
{
    public class ApiRequestDetails
    {
        public IHeaderDictionary Headers {get; private set;}
        public QueryString QueryString { get; private set; }
        public IDictionary<string, object> RouteValues {get; private set;}

        public ApiRequestDetails(IHeaderDictionary headers, QueryString queryString, IDictionary<string, object> routeValues)
        {
            Headers = headers;
            QueryString = queryString;
            RouteValues = routeValues;
        }
    }
}
