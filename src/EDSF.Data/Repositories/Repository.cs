using System.Linq.Expressions;
using EDSF.Core.Interfaces;
using EDSF.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace EDSF.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query()
    {
        var query = _dbSet.AsQueryable();
        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
        {
            query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
        }
        return query;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is ISoftDeletable sd && sd.IsDeleted)
            return null;
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await Query().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await Query().Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(T entity)
    {
        if (entity is ISoftDeletable sd)
        {
            sd.IsDeleted = true;
            sd.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
        return Task.CompletedTask;
    }
}
