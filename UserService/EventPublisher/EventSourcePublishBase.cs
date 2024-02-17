namespace UserService.EventPublisher;

public abstract class EventSourcePublishBase<T> : IEventPublishObservant<T>
{
    private readonly List<IEventPublishObserver<T>> _observers = [];
    public Task PublishAsync(T @event)
    {
        foreach (var observer in _observers)
        {
            observer.OnEventPublished(@event);
        }
        return Task.CompletedTask;
    }

    public void Subscribe(IEventPublishObserver<T> observer)
    {
        _observers.Add(observer);
    }
}