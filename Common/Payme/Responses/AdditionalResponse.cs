using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class AdditionalResponse
{
    [JsonPropertyName("field_name")] public string FieldName { get; set; }
}