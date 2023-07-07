using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CheckTransaction;

public class CheckTransactionRequest : BasePaymeRequest
{
    [JsonPropertyName("params")]
    public CheckTransactionRequestParameters Parameters { get; set; }
}