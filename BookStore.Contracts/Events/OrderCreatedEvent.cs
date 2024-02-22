using BookStore.EventObserver;

namespace OrderService.CreateOrder;

public record OrderCreatedEvent : EventBase
{
    public string OrderId { get; set; }
    public IEnumerable<string> BookIds { get; set; } = new[] { "" };
    public long TotalPrice { get; set; }
}