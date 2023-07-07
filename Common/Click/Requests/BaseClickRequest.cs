using System.Text.Json.Serialization;

namespace Common.Click.Requests;

public abstract class BaseClickRequest
{
    [JsonPropertyName("click_trans_id")]
    public long ClickTransactionId { get; set; }
    [JsonPropertyName("service_id")]
    public int ServiceId { get; set; }
    [JsonPropertyName("click_paydoc_id")]
    public long ClickPaydocId { get; set; }
    [JsonPropertyName("merchant_trans_id")]
    public string MerchantTransactionId { get; set; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("action")]
    public int Action { get; set; }
    [JsonPropertyName("error")]
    public int Error { get; set; }
    [JsonPropertyName("error_node")]
    public string ErrorNode { get; set; }
    [JsonPropertyName("sign_time")]
    public string SignTime { get; set; }
    [JsonPropertyName("sign_string")]
    public string SignString { get; set; }
}