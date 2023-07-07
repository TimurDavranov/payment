namespace Common.Payme.Enums;

public enum PaymeTransactionState
{
    EnterState = 0,
    WaitingConfirmation = 1,
    CompletedSuccessfully = 2,
    WaitingConfirmationCancelled = -1,
    CompletedSuccessfullyCancelled = -2
    // WaitingConfirmation = 0,
    // CreatingTransaction = 1,
    // Withdraw = 2,
    // ClosingTransaction = 3,
    // Paid = 4,
    // Pause = 20,
    // InCancelOrder = 21,
    // InClosingOrder = 30,
    // Canceled = 50
}