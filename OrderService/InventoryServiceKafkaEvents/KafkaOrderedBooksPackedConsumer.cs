using System.Text.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using Confluent.Kafka;
using UserService.Logout;

namespace OrderService.InventoryServiceEvents;

public class KafkaOrderedBooksPackedConsumer(KafkaOptions kafkaOptions, IEventPublishObservant observant)
    : KafkaConsumerBase<OrderedBooksPackedEvent>(kafkaOptions, observant)
{
    protected override string GetTopic(KafkaOptions kafkaOptions) => kafkaOptions.Topics.BooksPackedTopic;

    protected override void HandleException(Exception e)
    {
        // must have a fail tolerance mechanism (e.g. retry, dead letter queue, etc.) no time to implement it, for now just log the exception and panic
        Console.WriteLine(e);
        throw e;
    }
}