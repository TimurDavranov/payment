namespace Common.Payme.Enums;

public enum PaymeErrorCode
{
    NotAllowedMethod = -32300,
    JsonParsingError = -32700,
    RpcRequestFieldError = -32600,
    MethodNotFound = -32601,
    MethodExecutionPrivilegeError = -32504,
    SystemError = -32400,
    IncorrectAmount = -31001,
    TransactionNotFound = -31003,
    CantContinueOperation = -31008,
    IncorrectData = -31050, // до -31099
    ChequeEnded = -31007
}