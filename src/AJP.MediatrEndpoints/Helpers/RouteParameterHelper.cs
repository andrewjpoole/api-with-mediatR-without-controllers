using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AJP.MediatrEndpoints.Helpers
{
    public class RouteParameterHelper : IRouteParameterHelper
    {
        public List<string> GetRouteParamNamesFromPattern(string pattern)
        {
            var regexPattern = @"(?<=\{)[^}]*(?=\})";
            
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            var matches = regex.Matches(pattern);

            return matches.Select(x => x.Value).ToList();
        }
    }

    public interface IRouteParameterHelper
    {
        List<string> GetRouteParamNamesFromPattern(string pattern);
    }
}