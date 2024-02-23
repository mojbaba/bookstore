using System.Text.Json;
using BookStore.EventObserver;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace BookStore.EventLog.Kafka;

public abstract class KafkaConsumerBase<TEvent>(KafkaOptions kafkaOptions, IEventPublishObservant observant)
    : BackgroundService where TEvent : EventBase
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(() => StartConsuming(stoppingToken), stoppingToken,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }
    
    protected abstract string GetTopic(KafkaOptions kafkaOptions);
    
    protected abstract void HandleException(Exception e);

    private void StartConsuming(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            GroupId = kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe(GetTopic(kafkaOptions));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var @event =
                    JsonSerializer.Deserialize<TEvent>(consumeResult.Message.Value);


                observant.PublishAsync(@event).Wait(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
    }
}