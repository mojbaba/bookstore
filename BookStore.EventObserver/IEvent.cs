namespace BookStore.EventObserver;

public interface IEvent
{
    public Guid EventId { get; }
}