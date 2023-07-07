namespace Common.Click.Enums;

public enum ClickResponseAction
{
    Success = 0,
    SignCheckFailed = -1,
    IncorrectAmount = -2,
    ActionNotFound = -3,
    AlreadyPaid = -4,
    UserDoesNotExist = -5,
    TransactioDoesNotExist = -6,
    FailedToUpdateUser = -7,
    ErrorInClickRequest = -8,
    TransactionCancelled = -9
}