using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CheckPerform;

public class CheckPerformTransactionsParameters : BasePaymeRequestParameters
{
    [JsonPropertyName("account")] public AccountRequest AccountRequest { get; set; }
}