using BookStore.EventObserver;

namespace InventoryService.KafkaOrderEventsConsumer;

public static class ServiceRegisteration
{
    public static void AddKafkaOrderEventsConsumers(this IServiceCollection services)
    {
        services.AddSingleton<IEventPublishObserver, OrderCreatedEventObserver>();
        services.AddTransient<OrderCreatedEventHandler>();
        services.AddHostedService<KafkaOrderCreatedEventConsumer>();
        
        services.AddSingleton<IEventPublishObserver, OrderFailedEventObserver>();
        services.AddTransient<OrderFailedEventHandler>();
        services.AddHostedService<KafkaOrderFailedEventConsumer>();
    }
}