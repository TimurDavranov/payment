using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using Data.Base;

namespace Data.Entities;

[Table("cheques")]
public class Cheque : BaseEntity
{
    public decimal Amount { get; set; }
    public EpsSystem EpsSystem { get; set; }
    public ChequeStatus Status { get; set; }
    public required string ReturnUrl { get; set; }
    public required string CallBackUrl { get; set; }
    public bool CanCancel { get; set; }
    public required string UniqueId { get; set; }
    public Guid ClientId { get; set; }
    public long? PaymeTransactionId { get; set; }
    public virtual PaymeTransaction? PaymeTransaction { get; set; }
    public long? ClickTranactionId { get; set; }
    public virtual ClickTransaction? ClickTransaction { get; set; }
}