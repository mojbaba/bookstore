using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace OrderService.KafkaObserversForProduce;

public class OrderCreatedEventObserver(IEventLogProducer eventLogProducer, KafkaOptions kafkaOptions)
    : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderCreatedEvent orderCreatedEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<OrderCreatedEvent>(kafkaOptions.Topics.OrderCreatedTopic,
            orderCreatedEvent, CancellationToken.None);
    }
}