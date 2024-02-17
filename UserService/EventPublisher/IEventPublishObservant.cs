namespace UserService.EventPublisher;

public interface IEventPublishObservant<T>
{
    public Task PublishAsync(T @event);
    
    public void Subscribe(IEventPublishObserver<T> observer);
}