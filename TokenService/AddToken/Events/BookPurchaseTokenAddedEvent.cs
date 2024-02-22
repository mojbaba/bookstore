using BookStore.EventObserver;

namespace TokenService.AddToken;

public record BookPurchaseTokenAddedEvent : EventBase
{
    public string UserId { get; set; }
    public long AddedAmount { get; set; }
    
    public long UpdatedBalance { get; set; }
}