using System.Text.Json.Serialization;

namespace Common.Click.Responses;

public class BaseClickResponse
{
    [JsonPropertyName("click_trans_id")]
    public long ClickTransactionId { get; set; }
    [JsonPropertyName("merchant_trans_id")]
    public string MerchantTransactionId { get; set; }
    [JsonPropertyName("error")]
    public int Error { get; set; }
    [JsonPropertyName("error_node")]
    public string ErrorNote { get; set; }
}