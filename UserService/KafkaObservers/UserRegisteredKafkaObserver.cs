using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using UserService.Register.Events;

namespace UserService.KafkaObservers;

public class UserRegisteredKafkaObserver(IEventLogProducer eventLogProducer, IConfiguration configuration)
    : IEventPublishObserver
{
    private readonly string _topic = configuration["Kafka:Topics:UserRegisterTopic"];

    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserRegisteredEvent userRegisteredEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<UserRegisteredEvent>(_topic, userRegisteredEvent, CancellationToken.None);
    }
}