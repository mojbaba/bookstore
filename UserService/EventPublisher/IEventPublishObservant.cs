namespace UserService.EventPublisher;

public interface IEventPublishObservant
{
    public Task PublishAsync(EventBase @event);
    
    public void Subscribe(IEventPublishObserver observer);
}