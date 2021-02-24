using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AJP.MediatrEndpoints
{
    public class ApiRequestDetails
    {        
        public IDictionary<string, object> RouteValues {get; private set;}
        public IDictionary<string, string> QueryStringValues {get; private set;}
        public IDictionary<string, object> RequestHeaderValues {get; private set;}

        public ApiRequestDetails(IHeaderDictionary headers, QueryString queryString, IDictionary<string, object> routeValues)
        {
            RouteValues = new Dictionary<string, object>();
            foreach (var routeValue in routeValues)
            {
                RouteValues.Add(routeValue.Key, routeValue.Value);
            }

            RequestHeaderValues = new Dictionary<string, object>();
            foreach (var header in headers)
            {
                RequestHeaderValues.Add(header.Key, header.Value);
            }

            QueryStringValues = new Dictionary<string, string>();
            if (!queryString.HasValue)
                return;

            var queryStringSplit = queryString.Value.Replace("?", string.Empty).Split("&");
            foreach (var queryPair in queryStringSplit)
            {
                var pair = queryPair.Split("=");
                QueryStringValues.Add(pair[0], pair[1]);
            }
        }
    }
}
