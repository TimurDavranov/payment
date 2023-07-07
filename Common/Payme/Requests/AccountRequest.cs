using System.Text.Json.Serialization;

namespace Common.Payme.Requests;

public class AccountRequest
{
    [JsonPropertyName("phone")] public required string Phone { get; set; }
    
    [JsonPropertyName("chequeId")] public required string UniqueId { get; set; }
}