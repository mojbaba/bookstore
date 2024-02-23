using System.Text.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using Confluent.Kafka;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class KafkaBalanceDeductionFailedEventConsumer(KafkaOptions kafkaOptions, IEventPublishObservant observant)
    : KafkaConsumerBase<BalanceDeductionFailedEvent>(kafkaOptions, observant)
{
    protected override string GetTopic(KafkaOptions kafkaOptions) => kafkaOptions.Topics.BalanceDeductionFailedTopic;
    protected override void HandleException(Exception e)
    {
        // must have a fail tolerance mechanism (e.g. retry, dead letter queue, etc.) no time to implement it, for now just log the exception and panic
        Console.WriteLine(e);
        throw e;
    }
}