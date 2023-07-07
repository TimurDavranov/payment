using System.Text.Json.Serialization;

namespace Common.Click.Responses;

public class PrepareClickResponse : BaseClickResponse
{
    [JsonPropertyName("merchant_prepare_id")]
    public long MerchantPrepareId { get; set; }
}