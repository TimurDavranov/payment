using Common.Extentions;
using Common.Payme.Enums;
using Common.Payme.Responses;

namespace Common.Payme.Exeptions;

public class PaymeTranactionException : PaymeMerchantException
{
    public PaymeTranactionException(PaymeErrorCode errorCode, string text = "")
    {
        Error = new ErrorObject()
        {
            Code = (int)errorCode,
            Message = errorCode switch
            {
                PaymeErrorCode.IncorrectAmount => "Неверная сумма",
                PaymeErrorCode.CantContinueOperation => text.IsNull() ? "Невозможно выполнить операцию" : text,
                PaymeErrorCode.TransactionNotFound => "Трансакция ненайдена",
                _ => "Неверные данные"
            },
            Data = "tranaction",
        };
    }
}