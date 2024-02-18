namespace BookStore.EventObserver;

public record EventBase
{
    public Guid EventId { get; } = Guid.NewGuid();
}