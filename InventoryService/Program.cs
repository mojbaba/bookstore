using BookStore.RedisLock;
using InventoryService.AdminOperations;
using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace InventoryService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<InventoryServiceDbContext>((p, options) =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"));
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        builder.Services.AddSingleton(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("RedisConnection");
            return ConnectionMultiplexer.Connect(connectionString);
        });

        builder.Services.AddTransient<IDatabase>(provider =>
        {
            var multiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        builder.Services.AddTransient<IBookRepository, BookRepository>();
        builder.Services.AddTransient<IAdminOperationsService, AdminOperationsService>();
        builder.Services.AddTransient<IDistributedLock, RedisDistributedLock>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}