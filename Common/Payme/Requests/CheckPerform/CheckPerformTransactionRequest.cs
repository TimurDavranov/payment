using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CheckPerform;

public class CheckPerformTransactionRequest : BasePaymeRequest
{
    [JsonPropertyName("params")]
    public CheckPerformTransactionsParameters Parameters { get; set; }
}