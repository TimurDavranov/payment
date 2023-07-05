using Common.Enums;
using Common.Payme.Enums;
using Common.Payme.Exeptions;
using Common.Payme.Requests;
using Common.Payme.Responses;
using Data.Entities;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task<PaymeResponse<CheckPerformTransactionResponse>> CheckPerformTransaction(PaymeRequest request)
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
        var cheque = await _chequeRepository.GetAsync(s => s.UniqueId == request.Id,
            a => a.Include(s => s.PaymeTransaction));

        if (cheque is null || cheque.Status is not ChequeStatus.New)
            throw new PaymeTranactionException(PaymeErrorCode.IncorrectData);

        if (cheque.Amount != request.Amount)
            throw new PaymeTranactionException(PaymeErrorCode.IncorrectAmount);
        
        var tranaction = await _paymeRepository.AddAsync(new PaymeTransaction()
        {
            ChequeId = cheque.Id.Value,
            PaymeTransactionId = cheque.UniqueId,
            State = PaymeTransactionState.CreatingTransaction,
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

    public async Task<PaymeResponse<PerformTransactionResult>> PerformTransaction(string transactionId)
    {
        var tranaction = await _paymeRepository.GetAsync(s=>s.PaymeTransactionId == transactionId, a=>a.Include(s=>s.Cheque));
        
        if(tranaction is null ||
            tranaction.State is not PaymeTransactionState.WaitingConfirmation ||
            tranaction.Cheque is null ||
            tranaction.Cheque.Status is not ChequeStatus.Process)
            throw new PaymeTranactionException(PaymeErrorCode.TransactionNotFound);
        
        tranaction.PerformTransactionDateTime = DateTime.Now;
        tranaction.PerformTransactionTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        await _paymeRepository.UpdateAsync(tranaction);
        
        return new PaymeResponse<PerformTransactionResult>()
        {
            Result = new PerformTransactionResult()
            {
                PerformTime = tranaction.PerformTransactionTime,
                State = (int)PaymeResponseType.Close,
                TransactionId = transactionId
            }
        };
    }
    
    public async Task<PaymeResponse<CheckTransactionResult>> CheckTransaction(long id)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(_config.BasePaymentUrl + "/payme/transaction/");

        var api = await client.PostAsJsonAsync("check", id);
        var result = JsonConvert.DeserializeObject<PaymentResponse<PaymeTransactionDTO>>(await api.Content.ReadAsStringAsync());

        if (result.Type == PaymentResponseType.Cancelled || result.Type == PaymentResponseType.Failed)
        {
            return new PaymeResponse<CheckTransactionResult>
            {
                result = new CheckTransactionResult()
                {
                    transaction = id,
                    state = (int)PaymeTransactionState.Canceled,
                    reason = (int)PaymeTransactionResult.TransactionFailed
                }
            };
        }

        var bill = result.PaymentObject;

        return new PaymeResponse<CheckTransactionResult>()
        {
            result = new CheckTransactionResult()
            {
                create_time = bill.CreateTransactionTime,
                perform_time = bill.PerformTransactionTime,
                cancel_time = bill.CancelTransactionTime,
                transaction = bill.PaymeTransactionId,
                state = (int)bill.State,
                reason = (int)bill.TransactionResult
            }
        };
    }
    
    public async Task<PaymeResponse<CancelTransactionResult>> CancelTransaction(string id, PaymeTransactionResult reason)
    {
        var result = await _siteApiService.Cancel(id, reason);

        if (result.Type == PaymentResponseType.Failed)
        {
            throw new TransactionNotFoundException();
        }

        return new PaymeResponse<CancelTransactionResult>()
        {
            result = new CancelTransactionResult()
            {
                transaction = id,
                cancel_time = GetTimeStamp(),
                state = (int)PaymentResponseType.Cancelled

            }
        };
    }
    
    private long GetTimeStamp()
    {
        return ;
    }
}