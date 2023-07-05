using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class PaymeErrorResponse {
    [JsonPropertyName("error")]
    public ErrorObject Error { get; set; }
}