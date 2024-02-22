using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace OrderService.KafkaObserversForProduce;

public class OrderCreatedEventObserver(IEventLogProducer eventLogProducer, IConfiguration configuration)
    : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderCreatedEvent orderCreatedEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<OrderCreatedEvent>(configuration["Kafka:Topics:OrderCreatedTopic"],
            orderCreatedEvent, CancellationToken.None);
    }
}