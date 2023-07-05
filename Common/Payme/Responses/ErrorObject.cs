using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class ErrorObject 
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }
}