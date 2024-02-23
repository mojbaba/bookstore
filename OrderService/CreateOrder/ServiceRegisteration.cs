namespace OrderService.CreateOrder;

public static class ServiceRegisteration
{
    public static IServiceCollection AddCreateOrderServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateOrderService, CreateOrderService>();
        return services;
    }
}