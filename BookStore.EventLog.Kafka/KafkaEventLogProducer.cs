using System.Text.Json;
using BookStore.EventObserver;
using Confluent.Kafka;

namespace BookStore.EventLog.Kafka;

public class KafkaEventLogProducer : IEventLogProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventLogProducer(ProducerConfig producerConfig)
    {
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }
    
    public Task ProduceAsync<TEvent>(string channelName, IEvent @event, CancellationToken cancellationToken)
    {
        var eventJson = JsonSerializer.Serialize(@event);
        return _producer.ProduceAsync(channelName, new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = eventJson
        }, cancellationToken);
    }
}