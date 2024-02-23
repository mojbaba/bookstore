namespace OrderService.CreateOrder;

public static class ServiceRegisteration
{
    public static IServiceCollection AddCreateOrderServices(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryClient>((provider, client) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(configuration["Urls:InventoryService"]);
        });
        services.AddScoped<ICreateOrderService, CreateOrderService>();
        services.AddTransient<IInventoryClient, InventoryHttpClient>();
        return services;
    }
}