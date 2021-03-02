using System.Text.Json.Serialization;

namespace mediatr_test.RequestHandlers.Accounts
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