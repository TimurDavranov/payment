using System.Text.Json.Serialization;

namespace Common.Click.Requests;

public class CompleteClickRequest : BaseClickRequest
{
    [JsonPropertyName("merchant_prepare_id")]
    public int MerchantPrepareId { get; set; }
}