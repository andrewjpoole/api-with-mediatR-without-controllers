using System.Net.Http;

namespace AJP.MediatrEndpoints.Exceptions
{
    public class CustomHttpResponseException : HttpRequestException
    {
        public bool TriggerPostProcessor { get; }
        public bool TriggerErrorProcessor { get; }
        public int ResponseStatusCode { get; }
        public string ResponseBody { get; }

        public CustomHttpResponseException(
            string message, 
            bool triggerPostProcessor = true, 
            bool triggerErrorProcessor = false, 
            int responseStatusCode = 500, 
            string responseBody = ""):base(message)
        {
            TriggerPostProcessor = triggerPostProcessor;
            TriggerErrorProcessor = triggerErrorProcessor;
            ResponseStatusCode = responseStatusCode;
            ResponseBody = responseBody;
        }
    }
}