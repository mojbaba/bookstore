using InventoryService.Entities;

namespace InventoryService.QueryBooks;

public class BookQueryHandler(IBookRepository bookRepository) : IBookQueryHandler
{
    // it should have some pagination logic here
    public async Task<QueryBookResponse> HandleAsync(QueryBookRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<BookEntity> bookEntities;
        
        if (request.BookIds.Any())
        {
            bookEntities = await bookRepository.GetBooksByIdAsync(request.BookIds, cancellationToken);
        }
        else
        {
            bookEntities = await bookRepository.GetAllBooksAsync(cancellationToken);
        }

        return new QueryBookResponse
        {
            Books = bookEntities.Select(book => new Book
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price
            })
        };
    }
}