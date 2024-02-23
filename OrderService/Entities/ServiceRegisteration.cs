namespace OrderService.Entities;

public static class ServiceRegisteration
{
    public static IServiceCollection AddEntities(this IServiceCollection services)
    {
        services.AddTransient<IOrderRepository, OrderRepository>();
        return services;
    }
}