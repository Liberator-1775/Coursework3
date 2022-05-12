namespace Core.Base;

public interface IRepository<T>
{
    Task<T> CreateAsync(T entity);

    Task<T?> GetAsync(long id);

    IList<T> Get();

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}