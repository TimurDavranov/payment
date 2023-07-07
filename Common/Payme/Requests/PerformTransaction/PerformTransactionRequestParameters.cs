using System.Text.Json.Serialization;

namespace Common.Payme.Requests.PerformTransaction;

public class PerformTransactionRequestParameters
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}