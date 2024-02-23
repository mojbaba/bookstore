using Microsoft.EntityFrameworkCore;

namespace InventoryService.Entities;

public static class ServiceRegisteration
{
    public static IServiceCollection AddEntites(this IServiceCollection services)
    {
        services.AddDbContext<InventoryServiceDbContext>((p, options) =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"));
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
        
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookHistoryRepository, BookHistoryRepository>();

        return services;
    }
}