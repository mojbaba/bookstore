namespace BookStore.EventObserver;

public record EventBase : IEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
}