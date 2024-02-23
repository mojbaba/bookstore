using OrderService.EventLogConsumers;
using OrderService.InventoryServiceEvents;

namespace OrderService.InventoryServiceKafkaEvents;

public static class ServiceRegisteration
{
    public static IServiceCollection AddInventoryServiceKafkaEvents(this IServiceCollection services)
    {
        services.AddHostedService<KakfaOrderedBooksPackingFailedConsumer>();
        services.AddHostedService<KafkaOrderedBooksPackedConsumer>();

        services.AddTransient<OrderedBooksPackedEventHandler>();
        services.AddTransient<OrderedBooksPackingFailedEventHandler>();
        
        return services;
    }
}