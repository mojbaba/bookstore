using BookStore.TestingTools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using OrderService;
using OrderService.CreateOrder;
using OrderService.Entities;


namespace TokenService.IntegrationTest;

public class OrderServiceHostFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        try
        {
            Kafka = await TestInfrastructureHelper.CreateKafka(Configuration);
            Postgresql = await TestInfrastructureHelper.CreateDatabase(Configuration);
            Redis = await TestInfrastructureHelper.CreateRedis(Configuration);
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
        builder.ConfigureServices(services =>
        {
            RegisterConfiguration(services);
            RegisterInventoryClient(services);
        });
        
        base.ConfigureWebHost(builder);
    }


    private async Task ApplyMigration()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderServiceDbContext>();
        await context.Database.MigrateAsync();
    }

    private void RegisterConfiguration(IServiceCollection serviceCollection)
    {
        var descriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IConfiguration));

        if (descriptor != null)
            serviceCollection.Remove(descriptor);

        serviceCollection.AddSingleton<IConfiguration>(Configuration);
    }

    private void RegisterInventoryClient(IServiceCollection serviceCollection)
    {
        var descriptor = serviceCollection.Where(d => d.ServiceType == typeof(IInventoryClient));

        descriptor.ToList().ForEach(a =>
        {
            serviceCollection.Remove(a);
        });

        serviceCollection.AddSingleton<IInventoryClient>(InventoryClientMock.Object);
    }

    public PostgreSqlContainer Postgresql { get; private set; }

    public RedisContainer Redis { get; private set; }

    public KafkaContainer Kafka { get; set; }

    public IConfiguration Configuration { get; private set; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    public Mock<IInventoryClient> InventoryClientMock { get; set; } = new Mock<IInventoryClient>();

    public async Task DisposeAsync()
    {
        await Task.WhenAll(Redis.StopAsync(), Postgresql.StopAsync(), Kafka.StopAsync());

        await Redis.DisposeAsync();
        await Postgresql.DisposeAsync();
        await Kafka.DisposeAsync();
    }
}