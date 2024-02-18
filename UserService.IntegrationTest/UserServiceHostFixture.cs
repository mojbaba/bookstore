using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Moq;
using Testcontainers.Kafka;

namespace UserService.IntegrationTest;

public class UserServiceHostFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await CreateKafka();
        await CreateDatabase();
        await CreateRedis();
        await ApplyMigration();
    }

    private async Task CreateRedis()
    {
        Redis = new RedisBuilder()
            .WithImage("redis:6.0")
            .WithAutoRemove(true)
            .Build();

        await Redis.StartAsync();

        var configuration = this.Services.GetRequiredService<IConfiguration>();

        var connectionString = Redis.GetConnectionString();

        configuration["ConnectionStrings:RedisConnection"] = connectionString;
    }

    private async Task CreateDatabase()
    {
        Postgresql = new PostgreSqlBuilder()
            .WithImage("postgres:13")
            .WithDatabase("user_service")
            .WithUsername("user_service")
            .WithPassword("user_service")
            .WithAutoRemove(true)
            .Build();

        await Postgresql.StartAsync();

        var configuration = this.Services.GetRequiredService<IConfiguration>();

        var connectionString = Postgresql.GetConnectionString();

        configuration["ConnectionStrings:PostgreSqlConnection"] = connectionString;
    }

    private async Task CreateKafka()
    {
        Kafka = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:6.2.10")
            .WithAutoRemove(true)
            .Build();


        await Kafka.StartAsync();

        var configuration = this.Services.GetRequiredService<IConfiguration>();

        var connectionString = Kafka.GetBootstrapAddress();

        configuration["ConnectionStrings:KafkaConnection"] = connectionString;

        using var kafkaAdminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = connectionString
        }).Build();

        try
        {
            var topicsToCreate = new TopicSpecification[]
            {
                new TopicSpecification { Name = configuration["Kafka:UserLoginTopic"], ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification { Name = configuration["Kafka:UserLogoutTopic"] , ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification { Name = configuration["Kafka:UserRegisterTopic"], ReplicationFactor = 1, NumPartitions = 1 }
            };
            
            var existingTopics = kafkaAdminClient.GetMetadata(TimeSpan.FromMinutes(1));
            var existingTopicNames = existingTopics.Topics.Select(topic => topic.Topic);
            
            var topicsToCreateFiltered = topicsToCreate.Where(topic => !existingTopicNames.Contains(topic.Name)).ToArray();
            
            if (topicsToCreateFiltered.Length > 0)
            {
                await kafkaAdminClient.CreateTopicsAsync(topicsToCreateFiltered);
            }
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }


    private async Task ApplyMigration()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
        await context.Database.MigrateAsync();
    }

    public PostgreSqlContainer Postgresql { get; private set; }

    public RedisContainer Redis { get; private set; }

    public KafkaContainer Kafka { get; set; }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(Redis.StopAsync(), Postgresql.StopAsync());

        await Redis.DisposeAsync();
        await Postgresql.StopAsync();
    }
}