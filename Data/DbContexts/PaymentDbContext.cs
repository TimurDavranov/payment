using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;
using Data.Base;
using Data.Entities;

namespace Data.DbContexts;

public class PaymentDbContext : DbContext, IPaymentDbContext
{
    private readonly IHttpContextAccessor _httpContext;

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options, IHttpContextAccessor httpContext) :
        base(options)
    {
        _httpContext = httpContext;

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Add global query filter (soft delete) 
        Expression<Func<Audit, bool>> filter = s => !s.IsDeleted;
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.IsAssignableTo(typeof(Audit)))
            {
                var parameter = Expression.Parameter(entityType.ClrType);
                var body = ReplacingExpressionVisitor.Replace(filter.Parameters.First(), parameter, filter.Body);
                var lambda = Expression.Lambda(body, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }

        builder.Entity<Cheque>(ch => { ch.HasKey(s => s.Id); });

        builder.Entity<PaymeTransaction>(pt =>
        {
            pt.HasKey(s => s.Id);
            pt.HasOne(s => s.Cheque)
                .WithOne(s => s.PaymeTransaction)
                .HasForeignKey<PaymeTransaction>(s => s.ChequeId)
                .HasForeignKey<Cheque>(s => s.PaymeTransactionId);
        });

        builder.Entity<ClickTransaction>(ct =>
        {
            ct.HasKey(s => s.Id);
            ct.HasOne(s => s.Cheque)
                .WithOne(s => s.ClickTransaction)
                .HasForeignKey<ClickTransaction>(s => s.ChequeId)
                .HasForeignKey<Cheque>(s => s.PaymeTransactionId);
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditValue();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public int SaveChanges()
    {
        ApplyAuditValue();
        return base.SaveChanges();
    }

    private void ApplyAuditValue()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(s => s.Entity is Audit &&
                        s.State is EntityState.Added or EntityState.Deleted or EntityState.Modified);

        var userId = _httpContext.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        foreach (var entry in entities)
        {
            if (entry.State is EntityState.Added)
            {
                (entry.Entity as Audit).CreatedBy = userId;
                (entry.Entity as Audit).CreatedDate = DateTime.Now;
            }
            else if (entry.State is EntityState.Modified)
            {
                (entry.Entity as Audit).ModifiedBy = userId;
                (entry.Entity as Audit).ModifiedDate = DateTime.Now;
            }
            else if (entry.State is EntityState.Deleted)
            {
                (entry.Entity as Audit).IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }
    }

    public DbSet<Cheque> Cheques { get; set; }
    public DbSet<PaymeTransaction> PaymeTransactions { get; set; }
    public DbSet<ClickTransaction> ClickTransactions { get; set; }
}