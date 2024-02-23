using BookStore.Repository;

namespace TokenService.Entities;

public interface IBookPurchaseTokenHistoryRepository : IRepository<BookPurchaseTokenHistoryEntity>
{
    Task<IEnumerable<BookPurchaseTokenHistoryEntity>> GetItemsByOrderIdAsync(string orderId, CancellationToken cancellationToken);
}