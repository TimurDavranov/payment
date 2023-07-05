using Common.Payme.Responses;

namespace Common.Payme.Exeptions;

public class PaymeMerchantException : Exception
{
    public ErrorObject Error { get; set; }
}