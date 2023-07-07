using System.Text.Json.Serialization;

namespace Common.Payme.Requests.CreateTransaction;

public class CreateTransactionRequestParameters : BasePaymeRequestParameters
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("time")]
    public long Time { get; set; }
    [JsonPropertyName("account")]
    public AccountRequest Account { get; set; }
}