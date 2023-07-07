using Common.Click.Enums;
using Common.Click.Requests;
using Common.Click.Responses;
using Common.Enums;
using Common.Extentions;
using Data.Entities;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PaymentApi.Services;

public class ClickService
{
    private const string ClickSecretKey = "";

    private readonly IGenericRepository<Cheque> _chequeRepository;
    private readonly IGenericRepository<ClickTransaction> _clickRepository;

    public ClickService(IGenericRepository<Cheque> chequeRepository, IGenericRepository<ClickTransaction> clickRepository)
    {
        _chequeRepository = chequeRepository;
        _clickRepository = clickRepository;
    }
    
    public async Task<PrepareClickResponse> Prepare(PrepareClickRequest request)
    {
        var uniqueId = request.MerchantTransactionId;
        var md5Prepare =
            $"{request.ClickTransactionId}{request.ServiceId}{ClickSecretKey}{request.MerchantTransactionId}{request.Amount}{request.Action}{request.SignTime}"
                .ToMd5Hash();
        if (request.Action != (int)ClickRequestAction.Prepare)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.ActionNotFound,
                ErrorNote = "Метод не найден"
            };
        
        if (request.SignString != md5Prepare)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.SignCheckFailed,
                ErrorNote = "Неправильная сигнатура"
            };

        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.MerchantTransactionId,
            a => a.Include(s => s.ClickTransaction));
        if (cheque == null)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.TransactioDoesNotExist,
                ErrorNote = "Данной трансакции не существует"
            };

        if (cheque.Status is ChequeStatus.Cancel or ChequeStatus.Faild)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.TransactionCancelled,
                ErrorNote = "Данная трансакция отменена"
            };

        if (cheque.Status is ChequeStatus.Paying or ChequeStatus.Process or ChequeStatus.Paid)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.AlreadyPaid,
                ErrorNote = "Данная трансакция в процессе оплаты"
            };

        if (cheque.Amount != request.Amount)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.IncorrectAmount,
                ErrorNote = "Не правильная сумма"
            };

        if (cheque.ClickTransaction != null)
            return new PrepareClickResponse()
            {
                Error = (int)ClickResponseAction.Success,
                ErrorNote = "Success",
                ClickTransactionId = cheque.ClickTransaction.ClickTransactionId,
                MerchantPrepareId = cheque.ClickTransaction.Id!.Value,
                MerchantTransactionId = cheque.UniqueId
            };
        
        cheque.Status = ChequeStatus.Process;
        await _chequeRepository.UpdateAsync(cheque);
        
        var transaction = await _clickRepository.AddAsync(new ClickTransaction()
        {
            ClickTransactionId = request.ClickTransactionId,
            SignString = request.SignString,
            SignTime = request.SignTime,
            State = ClickTransactionState.WaitingConfirmation,
            ChequeId = cheque.Id!.Value,
            TransactionResult = TransactionCancellationReason.UnknownReason,
            ClickPaydocId = request.ClickPaydocId,
            PrepareTransactionDateTime = DateTime.Now
        });
        
        return new PrepareClickResponse()
        {
            Error = (int)ClickResponseAction.Success,
            ErrorNote = "Success",
            ClickTransactionId = transaction.ClickTransactionId,
            MerchantPrepareId = transaction.Id!.Value,
            MerchantTransactionId = cheque.UniqueId
        };
    }

    public async Task<CompleteClickResponse> Complete(CompleteClickRequest request)
    {
        var uniqueId = request.MerchantTransactionId;
        var md5Prepare =
            $"{request.ClickTransactionId}{request.ServiceId}{ClickSecretKey}{request.MerchantTransactionId}{request.Amount}{request.Action}{request.SignTime}"
                .ToMd5Hash();
        
        if (request.Action != (int)ClickRequestAction.Prepare)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.ActionNotFound,
                ErrorNote = "Метод не найден"
            };
        
        if (request.SignString != md5Prepare)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.SignCheckFailed,
                ErrorNote = "Неправильная сигнатура"
            };

        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.MerchantTransactionId,
            a => a.Include(s => s.ClickTransaction));
        if (cheque == null)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.TransactioDoesNotExist,
                ErrorNote = "Данной трансакции не существует"
            };

        if (cheque.Status is ChequeStatus.Cancel or ChequeStatus.Faild)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.TransactionCancelled,
                ErrorNote = "Данная трансакция отменена"
            };

        if (cheque.Status is ChequeStatus.Paying or ChequeStatus.Process or ChequeStatus.Paid)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.AlreadyPaid,
                ErrorNote = "Данная трансакция в процессе оплаты"
            };

        if (cheque.Amount != request.Amount)
            return new CompleteClickResponse()
            {
                Error = (int)ClickResponseAction.IncorrectAmount,
                ErrorNote = "Не правильная сумма"
            };
        
        cheque.Status = ChequeStatus.Paying;
        cheque.ClickTransaction.CompleteTransactionDateTime = DateTime.Now;
        cheque.EpsSystem = EpsSystem.Click;
        await _chequeRepository.UpdateAsync(cheque);

        return new CompleteClickResponse()
        {
            Error = (int)ClickResponseAction.Success,
            ErrorNote = "Success",
            ClickTransactionId = cheque.ClickTransaction.ClickTransactionId,
            MerchantTransactionId = cheque.UniqueId
        };
    }
}