namespace Common.Payme.Enums;

public enum PaymeTransactionState
{
    WaitingConfirmation = 0,
    CreatingTransaction = 1,
    Withdraw = 2,
    ClosingTransaction = 3,
    Paid = 4,
    Pause = 20,
    InCancelOrder = 21,
    InClosingOrder = 30,
    Canceled = 50
}