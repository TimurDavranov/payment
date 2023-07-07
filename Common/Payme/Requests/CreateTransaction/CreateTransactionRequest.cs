using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CreateTransaction;

public class CreateTransactionRequest : BasePaymeRequest
{
    [JsonPropertyName("params")]
    public CreateTransactionRequestParameters Parameters { get; set; }
}