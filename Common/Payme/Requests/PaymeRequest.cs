using System.Text.Json.Serialization;
using Common.Click.Enums;

namespace Common.Payme.Requests;

public class PaymeRequest
{
    [JsonPropertyName("method")] public required string RequestMethod { get; set; }

    [JsonPropertyName("params")] public required PaymeRequestParameters Parameters { get; set; }
}

public class PaymeRequestParameters
{
    [JsonPropertyName("amount")] public decimal Amount { get; set; }

    [JsonPropertyName("account")] public AccountRequest AccountRequest { get; set; }


    [JsonPropertyName("reason")] public TransactionCancellationReason Reason { get; set; }
}

public class AccountRequest
{
    [JsonPropertyName("phone")] public required string Phone { get; set; }
    
    [JsonPropertyName("chequeId")] public required string UniqueId { get; set; }
    
    [JsonPropertyName("id")] public string PaymeTransactionId {get;set;}
    
}