using BookStore.EventObserver;
using TokenService.BookPurchaseTokenHistoryHandlers;
using TokenService.RemoveToken;

namespace TokenService.ObserversForHistory;

public class TokenRemovedObserverForHistory(BookPurchaseTokenRemovedHandler purchaseTokenRemovedHandler)
    : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not BookPurchaseTokenRemovedEvent bookPurchaseTokenRemovedEvent)
        {
            return Task.CompletedTask;
        }

        return purchaseTokenRemovedHandler.HandleBookPurchaseTokenRemovedAsync(bookPurchaseTokenRemovedEvent, CancellationToken.None);
    }
}