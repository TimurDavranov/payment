using Common.Enums;
using Common.Models;
using Data.Entities;
using Data.Repositories;

namespace PaymentApi.Services;

public class ChequeService
{
    private readonly IGenericRepository<Cheque> _chequeRepository;
    private readonly IGenericRepository<PaymeTransaction> _paymeRepository;
    private readonly IGenericRepository<ClickTransaction> _clickRepository;
    public ChequeService(IGenericRepository<Cheque> chequeRepository, IGenericRepository<PaymeTransaction> paymeRepository, IGenericRepository<ClickTransaction> clickRepository)
    {
        _chequeRepository = chequeRepository;
        _paymeRepository = paymeRepository;
        _clickRepository = clickRepository;
    }

    public async Task Generate(PaymentRequest request)
    {
        var existed =
            await _chequeRepository.GetAsync(s => s.UniqueId == request.UniqueId && s.ClientId == request.ClientId);
        if (existed.Status != ChequeStatus.New)
        {
            throw new Exception("Чек с данным UniqueId уже существует");
        }

        var cheque = await _chequeRepository.AddAsync(new Cheque()
        {
            ReturnUrl = request.ReturlUrl,
            UniqueId = request.UniqueId,
            CallBackUrl = request.CallbackUrl,
            Amount = request.Amount,
            Status = ChequeStatus.New,
            CanCancel = true,
            ClientId = request.ClientId,
            EpsSystem = request.System
        });
    }

    public async Task Remove(string uniqueId) =>
        await _chequeRepository.DeleteAsync(s => s.UniqueId == uniqueId);
}