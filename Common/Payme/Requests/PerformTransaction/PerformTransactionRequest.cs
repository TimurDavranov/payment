using System.Text.Json.Serialization;

namespace Common.Payme.Requests.PerformTransaction;

public class PerformTransactionRequest : BasePaymeRequest
{
    [JsonPropertyName("params")]
    public PerformTransactionRequestParameters Parameters { get; set; }
}