using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class CreateTransactionResponse {
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
    [JsonPropertyName("transaction")]
    public string TransactionId { get; set; }
    [JsonPropertyName("state")]
    public int State { get; set; }
}