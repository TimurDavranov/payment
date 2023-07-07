using System.Text.Json.Serialization;

namespace Common.Payme.Requests;

public abstract class BasePaymeRequest
{
    [JsonPropertyName("method")]
    public string Method { get; set; }
}