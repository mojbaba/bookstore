using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Moq;

namespace UserService.IntegrationTest;

public class UserServiceHostFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
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

    private async Task ApplyMigration()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
        await context.Database.MigrateAsync();
    }

    public PostgreSqlContainer Postgresql { get; private set; }

    public RedisContainer Redis { get; private set; }
    

    public async Task DisposeAsync()
    {
        await Task.WhenAll(Redis.StopAsync(), Postgresql.StopAsync());
        
        await Redis.DisposeAsync();
        await Postgresql.StopAsync();
    }
}