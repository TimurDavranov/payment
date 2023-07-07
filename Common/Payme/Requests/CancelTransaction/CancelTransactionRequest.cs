using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CancelTransaction;

public class CancelTransactionRequest : BasePaymeRequest
{
    [JsonPropertyName("params")]
    public CancelTransactionRequestParameters Parameters { get; set; }
}