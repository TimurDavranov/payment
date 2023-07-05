using System.ComponentModel.DataAnnotations.Schema;
using Common.Click.Enums;
using Data.Base;

namespace Data.Entities;

[Table("click_transactions")]
public class ClickTransaction : BaseEntity
{
    public long ChequeId { get; set; }
    public virtual Cheque Cheque { get; }
    public required string ClickTransactionId { get; set; }
    public long ClickPaydocId { get; set; }
    public ClickTransactionState State { get; set; }
    public TransactionCancellationReason TransactionResult { get; set; }
    public DateTime CreateTransactionDateTime { get; set; } = DateTime.Now;
    public DateTime? PerformTransactionDateTime { get; set; }
    public DateTime? CancelTransactionDateTime { get; set; }
    public DateTime? PayedDate { get; set; }
    public string? SignString { get; set; }
    public string SignTime { get; set; }
}