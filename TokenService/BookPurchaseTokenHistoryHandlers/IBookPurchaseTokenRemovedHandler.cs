using TokenService.RemoveToken;

namespace TokenService.BookPurchaseTokenHistoryHandlers;

public interface IBookPurchaseTokenRemovedHandler
{
    Task HandleBookPurchaseTokenRemovedAsync(BookPurchaseTokenRemovedEvent @event, CancellationToken cancellationToken);
}