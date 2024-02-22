using Microsoft.EntityFrameworkCore;

namespace TokenService.Entities;

public class BookPurchaseTokenRepository(BookPurchaseTokenDbContext dbContext) : IBookPurchaseTokenRepository
{
    public Task<BookPurchaseTokenEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tokens.FirstOrDefaultAsync(a => a.UserId == id.ToString(), cancellationToken);
    }

    public Task<BookPurchaseTokenEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        return dbContext.Tokens.FirstOrDefaultAsync(a => a.UserId == id, cancellationToken);
    }

    public Task<BookPurchaseTokenEntity> CreateAsync(BookPurchaseTokenEntity entity,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Tokens.Add(entity).Entity);
    }

    public Task<BookPurchaseTokenEntity> UpdateAsync(BookPurchaseTokenEntity entity,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Tokens.Update(entity).Entity);
    }

    public Task<BookPurchaseTokenEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = dbContext.Tokens.FirstOrDefault(a => a.UserId == id.ToString());
        if (entity == null)
        {
            throw new KeyNotFoundException("Token not found");
        }

        return Task.FromResult(dbContext.Tokens.Remove(entity).Entity);
    }

    public Task<BookPurchaseTokenEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = dbContext.Tokens.FirstOrDefault(a => a.UserId == id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Token not found");
        }

        return Task.FromResult(dbContext.Tokens.Remove(entity).Entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}