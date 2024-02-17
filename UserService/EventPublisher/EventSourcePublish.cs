namespace UserService.EventPublisher;

public class EventSourcePublish : IEventPublishObservant
{
    private readonly List<IEventPublishObserver> _observers = [];

    public Task PublishAsync(EventBase @event)
    {
        foreach (var observer in _observers)
        {
            observer.OnEventPublished(@event);
        }

        return Task.CompletedTask;
    }

    public void Subscribe(IEventPublishObserver observer)
    {
        _observers.Add(observer);
    }
}