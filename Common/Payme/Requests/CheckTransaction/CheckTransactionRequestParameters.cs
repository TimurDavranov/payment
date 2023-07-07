using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CheckTransaction;

public class CheckTransactionRequestParameters
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}