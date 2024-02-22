using System.Text.Json;
using BookStore.EventObserver;
using Confluent.Kafka;

namespace BookStore.TestingTools;

public class KafkaFastConsumer<TEvent> where TEvent : IEvent
{
    private readonly ConsumerBuilder<Ignore, string> _consumer;
    private readonly string _topic;

    public KafkaFastConsumer(string bootstrapServers, string topic, string groupId)
    {
        _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset =  AutoOffsetReset.Earliest
        });
        _topic = topic;
    }
    
    public Task<List<TEvent>> ConsumeAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var messages = new List<TEvent>();
            using var consumer = _consumer.Build();
            consumer.Subscribe(_topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    var @event = JsonSerializer.Deserialize<TEvent>(consumeResult.Message.Value);
                    messages.Add(@event);
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }

            return messages;
        });
    }
}