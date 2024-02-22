using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using UserService.Login;

namespace UserService.KafkaObserversForProducer;

public class UserLoggedInKafkaObserver(IEventLogProducer eventLogProducer, IConfiguration configuration)
    : IEventPublishObserver
{
    private readonly string _topic = configuration["Kafka:Topics:UserLoginTopic"];
    
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not UserLoggedinEvent userLoginEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<UserLoggedinEvent>(_topic, userLoginEvent, CancellationToken.None);
    }
}