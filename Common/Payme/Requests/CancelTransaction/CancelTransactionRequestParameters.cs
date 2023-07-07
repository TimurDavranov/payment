using System.Text.Json.Serialization;
using Common.Payme.Enums;

namespace Common.Payme.Requests.CancelTransaction;

public class CancelTransactionRequestParameters
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("reason")]
    public TransactionCancellationReason Reason { get; set; }
}