using System.ComponentModel.DataAnnotations.Schema;
using Common.Payme.Enums;
using Data.Base;

namespace Data.Entities;

[Table("payme_transactions")]
public class PaymeTransaction : BaseEntity
{
    public long ChequeId { get; set; }
    public virtual Cheque Cheque { get; }
    public required string PaymeTransactionId { get; set; }
    public PaymeTransactionState State { get; set; }
    public TransactionCancellationReason? TransactionResult { get; set; }
    public DateTime CreateTransactionDateTime { get; set; }
    public DateTime? PerformTransactionDateTime { get; set; }
    public DateTime? CancelTransactionDateTime { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PayedDate { get; set; }
    public long CreateTransactionTime { get; set; } = 0;
    public long PerformTransactionTime { get; set; } = 0;
    public long CancelTransactionTime { get; set; } = 0;
}