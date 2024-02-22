using BookStore.EventObserver;
using TokenService.AddToken;
using TokenService.BookPurchaseTokenHistoryHandlers;

namespace TokenService.ObserversForHistory;

public class AddObserverForHistory(IBookPurchaseTokenAddedHandler bookPurchaseTokenAddedHandler) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not BookPurchaseTokenAddedEvent bookPurchaseTokenAddedEvent)
        {
            return Task.CompletedTask;
        }
        
        return bookPurchaseTokenAddedHandler.HandleBookPurchaseTokenAddedAsync(bookPurchaseTokenAddedEvent, CancellationToken.None);
    }
}