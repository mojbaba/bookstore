using BookStore.EventObserver;

namespace TokenService.RemoveToken;

public record BookPurchaseTokenRemovedEvent : EventBase
{
    public string UserId { get; set; }
    public long RemovedAmount { get; set; }
    public long UpdatedBalance { get; set; }
}