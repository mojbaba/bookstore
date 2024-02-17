namespace UserService.EventPublisher;

public interface IEventPublishObserver <in T>
{
    Task OnEventPublished(T @event);
}