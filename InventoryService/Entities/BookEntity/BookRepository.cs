using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities;

public class BookRepository(InventoryServiceDbContext dbContext) : IBookRepository
{
    public Task<BookEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Books.FirstOrDefaultAsync(a => a.Id == id.ToString(), cancellationToken);
    }

    public Task<BookEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        return dbContext.Books.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<BookEntity> CreateAsync(BookEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Books.Add(entity).Entity);
    }

    public Task<BookEntity> UpdateAsync(BookEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Books.Update(entity).Entity);
    }

    public async Task<BookEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Books.FirstOrDefaultAsync(a => a.Id == id.ToString());

        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }

        return dbContext.Books.Remove(entity).Entity;
    }

    public async Task<BookEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Books.FirstOrDefaultAsync(a => a.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }

        return dbContext.Books.Remove(entity).Entity;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookEntity>> GetAllBooksAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Books.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookEntity>> GetBooksByIdAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken)
    {
        return await dbContext.Books.AsNoTracking().Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);
    }
}