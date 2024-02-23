using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities;

public class BookHistoryRepository(InventoryServiceDbContext dbContext) : IBookHistoryRepository
{
    public Task<BookHistoryEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.BookHistories.FirstOrDefaultAsync(a => a.Id == id.ToString(), cancellationToken);
    }

    public Task<BookHistoryEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        return dbContext.BookHistories.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<BookHistoryEntity> CreateAsync(BookHistoryEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.BookHistories.Add(entity).Entity);
    }

    public Task<BookHistoryEntity> UpdateAsync(BookHistoryEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.BookHistories.Update(entity).Entity);
    }

    public async Task<BookHistoryEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.BookHistories.FirstOrDefaultAsync(a => a.Id == id.ToString());
        
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }
        
        return dbContext.BookHistories.Remove(entity).Entity;
    }

    public async Task<BookHistoryEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.BookHistories.FirstOrDefaultAsync(a => a.Id == id);
        
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }
        
        return dbContext.BookHistories.Remove(entity).Entity;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}