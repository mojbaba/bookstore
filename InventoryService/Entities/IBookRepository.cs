using BookStore.Repository;

namespace InventoryService.Entities;

public interface IBookRepository : IRepository<BookEntity>
{
     Task<IEnumerable<BookEntity>> GetAllBooksAsync(CancellationToken cancellationToken);
     
     Task<IEnumerable<BookEntity>> GetBooksByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
}