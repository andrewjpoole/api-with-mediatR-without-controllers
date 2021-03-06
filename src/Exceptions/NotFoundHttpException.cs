using System.Net.Http;

namespace AJP.MediatrEndpoints.Exceptions
{
    public class NotFoundHttpException : HttpRequestException
    {
        public string ResponseBody { get; }

        public NotFoundHttpException(string message, string responseBody = ""):base(message)
        {
            ResponseBody = responseBody;
        }
    }
}