namespace BookStore.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<T?> GetAsync(string id, CancellationToken cancellationToken);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<T> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<T> DeleteAsync(string id, CancellationToken cancellationToken);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}