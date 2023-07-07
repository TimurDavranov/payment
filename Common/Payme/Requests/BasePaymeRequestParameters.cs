using System.Text.Json.Serialization;

namespace Common.Payme.Requests;

public abstract class BasePaymeRequestParameters
{
    [JsonPropertyName("amount")] public decimal Amount { get; set; }
}