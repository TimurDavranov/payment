using System.Text.Json.Serialization;
using Common.Payme.Enums;

namespace Common.Payme.Responses;

public class CheckTransactionResponse
{
    [JsonPropertyName("transaction")]
    public string TransactionId { get; set; }
    [JsonPropertyName("state")]
    public int State { get; set; }
    [JsonPropertyName("reason")]
    public int Reason { get; set; }
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; } 
    [JsonPropertyName("perform_time")]
    public long PerformTime { get; set; }
    [JsonPropertyName("cancel_time")]
    public long CancelTime { get; set; }
}