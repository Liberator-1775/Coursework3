using Core.Base;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : Entity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _set;

    public BaseRepository(DbContext context, DbSet<T> set)
    {
        _context = context;
        _set = set;
    }
    
    public async Task<T> CreateAsync(T entity)
    {
        var createdEntity = await _set.AddAsync(entity);
        await _context.SaveChangesAsync();
        return createdEntity.Entity;
    }

    public async Task<T?> GetAsync(long id)
    {
        var entity = await _set.FindAsync(id);
        return entity;
    }

    public IList<T> Get()
    {
        return _set.ToList();
    }

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _context.SaveChangesAsync();
    }
}