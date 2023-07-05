using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IGenericRepository<TEntity>
{
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? expression = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    Task<long> CountAsync(Expression<Func<TEntity, bool>>? expression = null);
    IQueryable<TEntity> GetQueryable();
    
    Task UpdateAsync(TEntity entity);
    Task<TEntity> AddAsync(TEntity entity);
    Task DeleteAsync(Expression<Func<TEntity, bool>> expression);
    Task DeleteRangeAsync(Expression<Func<TEntity, bool>> expression);
    
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}