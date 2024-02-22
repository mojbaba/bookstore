using BookStore.EventObserver;

namespace BookStore.Contracts;

public record OrderedBooksPackingFailedEvent : EventBase
{
    public string OrderId { get; set; }
    public IEnumerable<string> BookIds { get; set; } = new[] { "" };
    public long TotalPrice { get; set; }
    public string Reason { get; set; }
}