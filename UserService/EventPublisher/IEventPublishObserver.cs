namespace UserService.EventPublisher;

public interface IEventPublishObserver
{
    Task OnEventPublished(EventBase @event);
}