using Microsoft.EntityFrameworkCore;

namespace TokenService.Entities;

public class BookPurchaseTokenHistoryRepository(BookPurchaseTokenDbContext dbContext)
    : IBookPurchaseTokenHistoryRepository
{
    public Task<BookPurchaseTokenHistoryEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.History.FirstOrDefaultAsync(a => a.Id == id.ToString(), cancellationToken);
    }

    public Task<BookPurchaseTokenHistoryEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        return dbContext.History.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<BookPurchaseTokenHistoryEntity> CreateAsync(BookPurchaseTokenHistoryEntity entity,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.History.Add(entity).Entity);
    }

    public Task<BookPurchaseTokenHistoryEntity> UpdateAsync(BookPurchaseTokenHistoryEntity entity,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.History.Update(entity).Entity);
    }

    public Task<BookPurchaseTokenHistoryEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = dbContext.History.FirstOrDefault(a => a.Id == id.ToString());
        if (entity == null)
        {
            throw new KeyNotFoundException("Token not found");
        }
        
        return Task.FromResult(dbContext.History.Remove(entity).Entity);
    }

    public Task<BookPurchaseTokenHistoryEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = dbContext.History.FirstOrDefault(a => a.Id == id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Token not found");
        }
        
        return Task.FromResult(dbContext.History.Remove(entity).Entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookPurchaseTokenHistoryEntity>> GetItemsByOrderIdAsync(string orderId, CancellationToken cancellationToken)
    {
        return await dbContext.History.Where(a => a.OrderId == orderId).ToListAsync(cancellationToken);
    }
}