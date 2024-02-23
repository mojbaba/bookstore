using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace InventoryService.KafkaOrderEventsConsumer;

public class KafkaOrderFailedEventConsumer(KafkaOptions options, IEventPublishObservant observant) :
    KafkaConsumerBase<OrderFailedEvent>(options, observant)
{
    protected override string GetTopic(KafkaOptions kafkaOptions) => kafkaOptions.Topics.OrderFailedTopic;

    protected override void HandleException(Exception e)
    {
        // must have a fail tolerance mechanism (e.g. retry, dead letter queue, etc.) no time to implement it, for now just log the exception and panic
        Console.WriteLine(e);
        throw e;
    }
}