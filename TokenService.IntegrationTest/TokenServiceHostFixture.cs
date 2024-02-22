using BookStore.TestingTools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using TokenService.Entities;

namespace TokenService.IntegrationTest;

public class TokenServiceHostFixture : WebApplicationFactory<Program>, IAsyncLifetime
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
        builder.ConfigureServices(RegisterConfiguration);
        base.ConfigureWebHost(builder);
    }


    private async Task ApplyMigration()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookPurchaseTokenDbContext>();
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