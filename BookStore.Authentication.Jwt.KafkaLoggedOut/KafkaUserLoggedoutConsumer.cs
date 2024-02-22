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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                var userLoggedOutEvent = JsonSerializer.Deserialize<UserLoggedOutEvent>(consumeResult.Message.Value);
                await _userLoggedOutHandler.Handle(userLoggedOutEvent, stoppingToken);
                Console.WriteLine(
                    $"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
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