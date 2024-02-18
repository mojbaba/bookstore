namespace BookStore.EventObserver;

public interface IEventPublishObserver
{
    Task OnEventPublished(EventBase @event);
}