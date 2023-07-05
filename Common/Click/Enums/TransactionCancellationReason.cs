namespace Common.Click.Enums;

public enum TransactionCancellationReason {
    MerchantNotFound = 1,
    MerchantServiceFailure = 2,
    TransactionFailed = 3,
    TransactionTimedOut = 4,
    MoneyReturn = 5,
    UnknownReason = 10
}