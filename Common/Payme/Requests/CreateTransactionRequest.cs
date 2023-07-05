using System.Text.Json.Serialization;

namespace Common.Payme.Requests;

public class CreateTransactionRequest
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("time")] public long Time { get; set; }
    [JsonPropertyName("amount")] public decimal Amount { get; set; }
    [JsonPropertyName("account")] public AccountRequest Aссount { get; set; }
}