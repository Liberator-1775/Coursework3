using Microsoft.EntityFrameworkCore;

namespace Core.Base;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _set;

    public BaseRepository(DbContext context, DbSet<T> set)
    {
        _context = context;
        _set = set;
    }
    
    public async Task CreateAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _context.SaveChangesAsync();
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