using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public interface IPaymentDbContext {
    
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    DbSet<Cheque> Cheques { get; set; }
    DbSet<PaymeTransaction> PaymeTransactions { get; set; }
    DbSet<ClickTransaction> ClickTransactions { get; set; }
}