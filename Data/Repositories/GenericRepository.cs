using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

using Data.DbContexts;

namespace Data.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly PaymentDbContext _context;
    
    public GenericRepository(PaymentDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        if(include != null)
        {
            query = include(query);
        }
        
        return await query.Where(expression).ToListAsync();
    }
    
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? expression = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        if(include != null)
        {
            query = include(query);
        }

        return await query.Where(expression).FirstOrDefaultAsync();
    }
    
    public async Task<long> CountAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        if(expression == null)
            return await _context.Set<TEntity>().CountAsync();
        
        return await _context.Set<TEntity>().CountAsync(expression);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        if (expression == null)
            return await _context.Set<TEntity>().AnyAsync();

        return await _context.Set<TEntity>().AnyAsync(expression);
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>().AsQueryable();
    }
    
    
    
    public async Task UpdateAsync(TEntity entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(expression);
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteRangeAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = await _context.Set<TEntity>().Where(expression).ToListAsync();
        _context.RemoveRange(entity);
        await _context.SaveChangesAsync();
    }
    
    
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}