using Common.Enums;
using Common.Payme.Enums;
using Common.Payme.Exeptions;
using Common.Payme.Requests;
using Common.Payme.Requests.CancelTransaction;
using Common.Payme.Requests.CheckPerform;
using Common.Payme.Requests.CheckTransaction;
using Common.Payme.Requests.CreateTransaction;
using Common.Payme.Requests.PerformTransaction;
using Common.Payme.Responses;
using Data.Entities;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using TransactionCancellationReason = Common.Click.Enums.TransactionCancellationReason;

namespace PaymentApi.Services;

public class PaymeService
{
    private readonly IGenericRepository<PaymeTransaction> _paymeRepository;
    private readonly IGenericRepository<Cheque> _chequeRepository;

    public PaymeService(IGenericRepository<PaymeTransaction> paymeRepository,
        IGenericRepository<Cheque> chequeRepository)
    {
        _paymeRepository = paymeRepository;
        _chequeRepository = chequeRepository;
    }

    public async Task<PaymeResponse<CheckPerformTransactionResponse>> CheckPerformTransaction(
        CheckPerformTransactionRequest request)
    {
        var cheque = await _chequeRepository.GetAsync(s =>
            s.UniqueId == request.Parameters.AccountRequest.UniqueId && s.Amount == request.Parameters.Amount);
        if (cheque is null)
            return new PaymeResponse<CheckPerformTransactionResponse>()
            {
                Result = new CheckPerformTransactionResponse()
                {
                    Allow = false,
                    Additional = new AdditionalResponse()
                    {
                        FieldName = "У поставщика данный чек ненайден!"
                    }
                }
            };

        if (cheque.Amount != request.Parameters.Amount)
            return new PaymeResponse<CheckPerformTransactionResponse>()
            {
                Result = new CheckPerformTransactionResponse()
                {
                    Allow = false,
                    Additional = new AdditionalResponse()
                    {
                        FieldName = "У поставщика чек c текущей суммой ненайден!"
                    }
                }
            };

        if (cheque.Status is not ChequeStatus.New)
            return new PaymeResponse<CheckPerformTransactionResponse>()
            {
                Result = new CheckPerformTransactionResponse()
                {
                    Allow = false,
                    Additional = new AdditionalResponse()
                    {
                        FieldName = "Данный платеж осуществляется с другой електронной платежной системы!"
                    }
                }
            };

        cheque.EpsSystem = EpsSystem.Payme;
        await _chequeRepository.UpdateAsync(cheque);

        var response = new PaymeResponse<CheckPerformTransactionResponse>()
        {
            Result = new CheckPerformTransactionResponse()
            {
                Allow = true
            }
        };

        return response;
    }

    public async Task<PaymeResponse<CreateTransactionResponse>> CreateTransaction(CreateTransactionRequest request)
    {
        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.Parameters.Account.UniqueId,
            a => a.Include(s => s.PaymeTransaction));

        if (cheque is null || cheque.Status is not ChequeStatus.New)
            throw new PaymeTranactionException(PaymeErrorCode.IncorrectData);

        if (cheque.Amount != request.Parameters.Amount)
            throw new PaymeTranactionException(PaymeErrorCode.IncorrectAmount);

        var tranaction = await _paymeRepository.AddAsync(new PaymeTransaction()
        {
            ChequeId = cheque.Id.Value,
            PaymeTransactionId = cheque.UniqueId,
            State = PaymeTransactionState.WaitingConfirmation,
            TransactionResult = null,
            CreateTransactionTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
            CreateTransactionDateTime = DateTime.Now
        });

        cheque.Status = ChequeStatus.Process;
        await _chequeRepository.UpdateAsync(cheque);

        return new PaymeResponse<CreateTransactionResponse>()
        {
            Result = new CreateTransactionResponse()
            {
                CreateTime = cheque.PaymeTransaction.CreateTransactionTime,
                State = (int)PaymeResponseType.Success,
                TransactionId = cheque.UniqueId
            }
        };
    }

    public async Task<PaymeResponse<PerformTransactionResult>> PerformTransaction(PerformTransactionRequest request)
    {
        var tranaction = await _paymeRepository.GetAsync(s => s.PaymeTransactionId == request.Parameters.Id,
            a => a.Include(s => s.Cheque));

        if (tranaction is null ||
            tranaction.State is not PaymeTransactionState.WaitingConfirmation ||
            tranaction.Cheque is null ||
            tranaction.Cheque.Status is not ChequeStatus.Process)
            throw new PaymeTranactionException(PaymeErrorCode.TransactionNotFound);

        tranaction.PerformTransactionDateTime = DateTime.Now;
        tranaction.PerformTransactionTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        tranaction.Cheque.Status = ChequeStatus.Paying;
        tranaction.Cheque.EpsSystem = EpsSystem.Payme;
        await _paymeRepository.UpdateAsync(tranaction);

        return new PaymeResponse<PerformTransactionResult>()
        {
            Result = new PerformTransactionResult()
            {
                PerformTime = tranaction.PerformTransactionTime,
                State = (int)PaymeResponseType.Close,
                TransactionId = request.Parameters.Id
            }
        };
    }

    public async Task<PaymeResponse<CheckTransactionResponse>> CheckTransaction(CheckTransactionRequest request)
    {
        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.Parameters.Id,
            a => a.Include(s => s.PaymeTransaction));
        if (cheque == null || cheque.PaymeTransaction == null)
            throw new PaymeTranactionException(PaymeErrorCode.TransactionNotFound);

        if (cheque.PaymeTransaction.State is not PaymeTransactionState.CompletedSuccessfully
            or PaymeTransactionState.WaitingConfirmation)
            throw new PaymeTranactionException(PaymeErrorCode.CantContinueOperation);

        if (cheque.PaymeTransaction.State is PaymeTransactionState.CompletedSuccessfully)
            return new PaymeResponse<CheckTransactionResponse>()
            {
                Result = new CheckTransactionResponse()
                {
                    TransactionId = request.Parameters.Id,
                    State = (int)PaymeTransactionState.CompletedSuccessfully,
                    PerformTime = cheque.PaymeTransaction.PerformTransactionTime
                }
            };

        if ((DateTime.Now - cheque.PaymeTransaction.CreateTransactionDateTime).Hours > 10)
        {
            await CancelTransaction(new CancelTransactionRequest()
            {
                Parameters = new CancelTransactionRequestParameters()
                {
                    Id = request.Parameters.Id,
                    Reason = Common.Payme.Enums.TransactionCancellationReason.TransactionTimedOut
                }
            });
            throw new PaymeTranactionException(PaymeErrorCode.CantContinueOperation, "Время оплаты трансакции истекло");
        }

        cheque.PaymeTransaction.State = PaymeTransactionState.CompletedSuccessfully;
        cheque.Status = ChequeStatus.Paying;
        
        await _chequeRepository.UpdateAsync(cheque);

        return new PaymeResponse<CheckTransactionResponse>()
        {
            Result = new CheckTransactionResponse()
            {
                State = (int)cheque.Status,
                PerformTime = cheque.PaymeTransaction.PerformTransactionTime,
                TransactionId = request.Parameters.Id,
            }
        };
    }

    public async Task<PaymeResponse<CancelTransactionResponse>> CancelTransaction(CancelTransactionRequest request)
    {
        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.Parameters.Id,
            a => a.Include(s => s.PaymeTransaction));

        if (cheque == null || cheque.PaymeTransaction == null)
            throw new PaymeTranactionException(PaymeErrorCode.TransactionNotFound);

        if (cheque.PaymeTransaction.State == PaymeTransactionState.WaitingConfirmation)
        {
            cheque.Status = ChequeStatus.Cancel;
            cheque.PaymeTransaction.State = PaymeTransactionState.WaitingConfirmationCancelled;
            cheque.PaymeTransaction.CancelTransactionTime =
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            cheque.PaymeTransaction.CancelTransactionDateTime = DateTime.Now;
            await _chequeRepository.UpdateAsync(cheque);
            return new PaymeResponse<CancelTransactionResponse>()
            {
                Result = new CancelTransactionResponse()
                {
                    State = (int)PaymeTransactionState.WaitingConfirmationCancelled,
                    TransactionId = request.Parameters.Id,
                    CancelTime = cheque.PaymeTransaction.CancelTransactionTime
                }
            };
        }

        if (!cheque.CanCancel)
            throw new PaymeTranactionException(PaymeErrorCode.ChequeEnded);

        if (cheque.PaymeTransaction.State is PaymeTransactionState.CompletedSuccessfully)
        {
            cheque.Status = ChequeStatus.Cancel;
            cheque.PaymeTransaction.State = PaymeTransactionState.CompletedSuccessfullyCancelled;
            cheque.PaymeTransaction.CancelTransactionTime =
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            cheque.PaymeTransaction.CancelTransactionDateTime = DateTime.Now;
            await _chequeRepository.UpdateAsync(cheque);

            return new PaymeResponse<CancelTransactionResponse>()
            {
                Result = new CancelTransactionResponse()
                {
                    TransactionId = request.Parameters.Id,
                    CancelTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                    State = (int)PaymeTransactionState.CompletedSuccessfullyCancelled
                }
            };
        }

        return new PaymeResponse<CancelTransactionResponse>()
        {
            Result = new CancelTransactionResponse()
            {
                CancelTime = cheque.PaymeTransaction.CancelTransactionTime,
                State = (int)cheque.PaymeTransaction.State,
                TransactionId = request.Parameters.Id
            }
        };
    }
}