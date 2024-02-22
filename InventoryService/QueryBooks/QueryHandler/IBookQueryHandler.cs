namespace InventoryService.QueryBooks;

public interface IBookQueryHandler
{
    Task<QueryBookResponse> HandleAsync(QueryBookRequest request,CancellationToken cancellationToken);
}