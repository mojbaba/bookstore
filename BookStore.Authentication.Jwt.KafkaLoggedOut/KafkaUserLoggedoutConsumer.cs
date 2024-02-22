using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using UserService.Logout;

namespace BookStore.Authentication.Jwt.KafkaLoggedOut;

public class KafkaUserLoggedoutConsumer : BackgroundService
{
    private readonly string _bootstrapServers;
    private readonly string _groupId;
    private readonly string _topic;
    private readonly KafkaUserLoggedOutHandler _userLoggedOutHandler;

    public KafkaUserLoggedoutConsumer(KafkaUserLoggedOutOptions options, KafkaUserLoggedOutHandler userLoggedOutHandler)
    {
        _bootstrapServers = options.BootstrapServers;
        _groupId = options.GroupId;
        _topic = options.Topic;
        _userLoggedOutHandler = userLoggedOutHandler;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(()=>StartConsuming(stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private void StartConsuming(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe(_topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var userLoggedOutEvent =
                    JsonSerializer.Deserialize<UserLoggedOutEvent>(consumeResult.Message.Value);
                _userLoggedOutHandler.Handle(userLoggedOutEvent, stoppingToken).Wait(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            catch (Exception e)
            {
            }
        }
    }
}