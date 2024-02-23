using BookStore.EventObserver;

namespace OrderService.CreateOrder;

public record OrderCreatedEvent : EventBase
{
    public required string OrderId { get; set; }
    public required string UserId { get; set; }
    public IEnumerable<string> BookIds { get; set; } = new[] { "" };
    public required long TotalPrice { get; set; }
}