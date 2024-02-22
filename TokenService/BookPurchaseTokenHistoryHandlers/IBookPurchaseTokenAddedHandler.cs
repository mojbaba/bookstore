using TokenService.AddToken;

namespace TokenService.BookPurchaseTokenHistoryHandlers;

public interface IBookPurchaseTokenAddedHandler
{
    Task HandleBookPurchaseTokenAddedAsync(BookPurchaseTokenAddedEvent @event, CancellationToken cancellationToken);
}