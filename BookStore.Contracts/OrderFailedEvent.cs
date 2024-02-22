using BookStore.EventObserver;

namespace OrderService.CreateOrder;

public record OrderFailedEvent : EventBase
{
    public string OrderId { get; set; }
    public IEnumerable<string> BookIds { get; set; } = new[] { "" };
    public long TotalPrice { get; set; }
    public string Reason { get; set; }
}