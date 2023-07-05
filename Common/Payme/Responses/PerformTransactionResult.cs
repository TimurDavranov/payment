using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class PerformTransactionResult
{
    [JsonPropertyName("perform_time")] public long PerformTime { get; set; }
    [JsonPropertyName("state")] public int State { get; set; }
    [JsonPropertyName("transaction")] public string TransactionId { get; set; }
}