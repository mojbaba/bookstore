using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using UserService.Logout;

namespace UserService.KafkaObservers;

public class UserLoggedOutKafkaObserver(IEventLogProducer eventLogProducer, IConfiguration configuration)
    : IEventPublishObserver
{
    private readonly string _topic = configuration["Kafka:Topics:UserLogoutTopic"];

    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserLoggedOutEvent userLogoutEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<UserLoggedOutEvent>(_topic, userLogoutEvent, CancellationToken.None);
    }
}