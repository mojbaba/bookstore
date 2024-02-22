using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace BookStore.TestingTools;

public static class TestInfrastructureHelper
{
    public  static async Task<PostgreSqlContainer> CreateDatabase(IConfiguration configuration)
    {
        var postgresql = new PostgreSqlBuilder()
            .WithImage("postgres:13")
            .WithAutoRemove(true)
            .Build();

        await postgresql.StartAsync();

        var connectionString = postgresql.GetConnectionString();

        configuration["ConnectionStrings:PostgreSqlConnection"] = connectionString;

        return postgresql;
    }
    public static async Task<RedisContainer> CreateRedis(IConfiguration configuration)
    {
        var redis = new RedisBuilder()
            .WithImage("redis:6.0")
            .WithAutoRemove(true)
            .Build();

        await redis.StartAsync();

        var connectionString = redis.GetConnectionString();

        configuration["ConnectionStrings:RedisConnection"] = connectionString;

        return redis;
    }
    public static async Task<KafkaContainer> CreateKafka(IConfiguration configuration)
    {
        var kafka = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:6.2.10")
            .WithAutoRemove(true)
            .Build();


        await kafka.StartAsync();

        var connectionString = kafka.GetBootstrapAddress();

        configuration["Kafka:BootstrapServers"] = connectionString;

        using var kafkaAdminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = connectionString
        }).Build();

        try
        {
            var topicsToCreate = new TopicSpecification[]
            {
                new TopicSpecification
                    { Name = configuration["Kafka:Topics:UserLoginTopic"], ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification
                    { Name = configuration["Kafka:Topics:UserLogoutTopic"], ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification
                    { Name = configuration["Kafka:Topics:UserRegisterTopic"], ReplicationFactor = 1, NumPartitions = 1 }
            };

            var existingTopics = kafkaAdminClient.GetMetadata(TimeSpan.FromMinutes(1));
            var existingTopicNames = existingTopics.Topics.Select(topic => topic.Topic);

            var topicsToCreateFiltered =
                topicsToCreate.Where(topic => !existingTopicNames.Contains(topic.Name)).ToArray();

            if (topicsToCreateFiltered.Length > 0)
            {
                await kafkaAdminClient.CreateTopicsAsync(topicsToCreateFiltered);
            }
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }

        return kafka;
    }
}