namespace BookStore.EventObserver;

internal class EventSourcePublish : IEventPublishObservant
{
    private readonly List<IEventPublishObserver> _observers = [];

    public Task PublishAsync(EventBase @event)
    {
        return Task.WhenAll(_observers.Select(ob => ob.OnEventPublished(@event)));
    }

    public void Subscribe(IEventPublishObserver observer)
    {
        _observers.Add(observer);
    }
}