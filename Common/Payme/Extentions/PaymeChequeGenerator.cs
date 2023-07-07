using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Models;
using Common.Payme.Requests;

namespace Common.Payme.Extentions;

public static class PaymeChequeGenerator
{
    private const string PaycomUrl = "https://checkout.paycom.uz/";
    private const string MerchantId = "";
    
    public static string Generate(PaymentRequest request)
    {
        var result = new StringBuilder(PaycomUrl);
        result.Append($"m={MerchantId}");
        result.Append($"ac={JsonSerializer.Serialize(new AccountRequest()
        {
            Phone = request.Phone,
            UniqueId = request.UniqueId
        })}");
        result.Append($"a={request.Amount}");
        result.Append($"l={request.LanguageCode}");
        result.Append($"c={request.ReturlUrl}");
        result.Append($"ct=2000");
        result.Append($"cr={request.CurrencyCode}");
        return PaycomUrl + Convert.ToBase64String(Encoding.UTF8.GetBytes(result.ToString()));
    }
}