using System.Text.Json.Serialization;

namespace Common.Click.Responses;

public class CompleteClickResponse : BaseClickResponse
{
    [JsonPropertyName("merchant_confirm_id")]
    public int MerchantConfirmId { get; set; }
}