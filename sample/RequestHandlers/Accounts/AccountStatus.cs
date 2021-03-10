using System.Text.Json.Serialization;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountStatus
    {
        Any,
        Blocked,
        Unblocked,
        New,
        Closed
    }
}