using System.Text.Json.Serialization;

namespace Common.Payme.Responses;

public class PaymeResponse
{
    public PaymeErrorResponse ErrorResponse { get; set; }

    public string ResultResponse { get; set; }
}

public class PaymeResponse<T>
{
    [JsonPropertyName("result")]
    public T Result { get; set; }
}