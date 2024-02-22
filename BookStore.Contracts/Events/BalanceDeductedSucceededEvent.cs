using BookStore.EventObserver;

namespace BookStore.Contracts;

public record BalanceDeductedSucceededEvent : EventBase
{
    public string OrderId { get; set; }
    public IEnumerable<string> BookIds { get; set; } = new[] { "" };
    public long TotalPrice { get; set; }
}