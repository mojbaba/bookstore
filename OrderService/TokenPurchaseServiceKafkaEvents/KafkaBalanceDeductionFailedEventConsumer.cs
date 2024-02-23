using System.Text.Json;
using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using Confluent.Kafka;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class KafkaBalanceDeductionFailedEventConsumer(KafkaOptions kafkaOptions, BalanceDeductionFailedEventHandler handler)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(() => StartConsuming(stoppingToken), stoppingToken,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void StartConsuming(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            GroupId = kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe(kafkaOptions.Topics.BalanceDeductionFailedTopic);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var @event =
                    JsonSerializer.Deserialize<BalanceDeductionFailedEvent>(consumeResult.Message.Value);


                handler.HandleAsync(@event, stoppingToken).Wait(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            catch (Exception e)
            {
                // must have a fail tolerance mechanism (e.g. retry, dead letter queue, etc.) no time to implement it, for now just log the exception and panic
                Console.WriteLine(e);
                throw;
            }
        }
    }
}