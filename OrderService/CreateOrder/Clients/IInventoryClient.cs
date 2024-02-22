using InventoryService.QueryBooks;

namespace OrderService.CreateOrder;

public interface IInventoryClient
{
     Task<QueryBookResponse> QueryBooksAsync(QueryBookRequest request, CancellationToken cancellationToken);
}