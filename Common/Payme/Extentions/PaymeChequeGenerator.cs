using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Payme.Requests;

namespace Common.Payme.Extentions;

public static class PaymeChequeGenerator
{
    private const string PaycomUrl = "https://checkout.paycom.uz/";
    
    public static string Generate(string url, string merchantId, PaymeRequestParameters parameters, decimal amount, string returnUrl, string currenctIsoCode = "860")
    {
        var result = new StringBuilder(PaycomUrl);
        result.Append($"m={merchantId}");
        result.Append($"ac={JsonSerializer.Serialize(parameters)}");
        result.Append($"a={amount}");
        result.Append($"c={returnUrl}");
        result.Append($"ct=1000");
        result.Append($"cr=")
    }
}