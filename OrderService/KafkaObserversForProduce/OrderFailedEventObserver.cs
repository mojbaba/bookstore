using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace OrderService.KafkaObserversForProduce;

public class OrderFailedEventObserver(IEventLogProducer eventLogProducer, KafkaOptions kafkaOptions)
    : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderFailedEvent orderFailedEvent)
        {
            return Task.CompletedTask;
        }

        return eventLogProducer.ProduceAsync<OrderFailedEvent>(kafkaOptions.Topics.OrderFailedTopic,
            orderFailedEvent, CancellationToken.None);
    }
}