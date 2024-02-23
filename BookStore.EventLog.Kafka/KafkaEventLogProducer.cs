using System.Text.Json;
using BookStore.EventObserver;
using Confluent.Kafka;

namespace BookStore.EventLog.Kafka;

public class KafkaEventLogProducer : IEventLogProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventLogProducer(KafkaOptions kafkaOptions)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.BootstrapServers
        };
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public Task ProduceAsync<TEvent>(string channelName, TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        var eventJson = JsonSerializer.Serialize<TEvent>(@event);
        return _producer.ProduceAsync(channelName, new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = eventJson
        }, cancellationToken);
    }
}