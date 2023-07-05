using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class CheckPerformTransactionResponse
{
    [JsonPropertyName("allow")]
    public bool Allow { get; set; }
    
    [JsonPropertyName("additional")]
    public AdditionalResponse Additional {get;set;}
}