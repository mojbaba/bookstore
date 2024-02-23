using BookStore.EventObserver;

namespace OrderService.KafkaObserversForProduce;

public static class ServiceRegisteration
{
    public static IServiceCollection AddKafkaObserversForProduce(this IServiceCollection services)
    {
        services.AddTransient<IEventPublishObserver, OrderCreatedEventObserver>();
        services.AddTransient<IEventPublishObserver, OrderFailedEventObserver>();

        return services;
    }
}