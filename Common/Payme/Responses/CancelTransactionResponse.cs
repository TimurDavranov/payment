using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class CancelTransactionResponse
{
    [JsonPropertyName("transaction")]
    public string TransactionId { get; set; }
    [JsonPropertyName("state")]
    public int State { get; set; }
    [JsonPropertyName("cancel_time")]
    public long CancelTime { get; set; }
}