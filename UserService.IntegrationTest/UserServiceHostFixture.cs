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
        try
        {
            await CreateKafka();
            await CreateDatabase();
            await CreateRedis();
            await ApplyMigration();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(RegisterConfiguration);
        base.ConfigureWebHost(builder);
    }

    private async Task CreateRedis()
    {
        Redis = new RedisBuilder()
            .WithImage("redis:6.0")
            .WithAutoRemove(true)
            .Build();

        await Redis.StartAsync();

        var connectionString = Redis.GetConnectionString();

        Configuration["ConnectionStrings:RedisConnection"] = connectionString;
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

        var connectionString = Postgresql.GetConnectionString();

        Configuration["ConnectionStrings:PostgreSqlConnection"] = connectionString;
    }

    private async Task CreateKafka()
    {
        Kafka = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:6.2.10")
            .WithAutoRemove(true)
            .Build();


        await Kafka.StartAsync();

        var connectionString = Kafka.GetBootstrapAddress();

        Configuration["Kafka:BootstrapServers"] = connectionString;

        using var kafkaAdminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = connectionString
        }).Build();

        try
        {
            var topicsToCreate = new TopicSpecification[]
            {
                new TopicSpecification
                    { Name = Configuration["Kafka:UserLoginTopic"], ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification
                    { Name = Configuration["Kafka:UserLogoutTopic"], ReplicationFactor = 1, NumPartitions = 1 },
                new TopicSpecification
                    { Name = Configuration["Kafka:UserRegisterTopic"], ReplicationFactor = 1, NumPartitions = 1 }
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
    }


    private async Task ApplyMigration()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
        await context.Database.MigrateAsync();
    }

    private void RegisterConfiguration(IServiceCollection serviceCollection)
    {
        var descriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IConfiguration));

        if (descriptor != null)
            serviceCollection.Remove(descriptor);

        serviceCollection.AddSingleton<IConfiguration>(Configuration);
    }

    public PostgreSqlContainer Postgresql { get; private set; }

    public RedisContainer Redis { get; private set; }

    public KafkaContainer Kafka { get; set; }

    public IConfiguration Configuration { get; private set; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();


    public async Task DisposeAsync()
    {
        await Task.WhenAll(Redis.StopAsync(), Postgresql.StopAsync());

        await Redis.DisposeAsync();
        await Postgresql.StopAsync();
        await Kafka.StopAsync();
    }
}