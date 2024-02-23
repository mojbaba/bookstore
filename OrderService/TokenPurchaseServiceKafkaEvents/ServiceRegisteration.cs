using BookStore.EventObserver;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public static class ServiceRegisteration
{
    public static IServiceCollection AddTokenPurchaseServiceKafkaEvents(this IServiceCollection services)
    {
        services.AddTransient<BalanceDeductedSucceededEventHandler>();
        services.AddTransient<BalanceDeductionFailedEventHandler>();

        services.AddHostedService<KafkaBalanceDeductedSucceededEventConsumer>();
        services.AddHostedService<KafkaBalanceDeductionFailedEventConsumer>();

        services.AddSingleton<IEventPublishObserver, BalanceDeductedSucceededEventObserver>();
        services.AddSingleton<IEventPublishObserver, BalanceDeductionFailedEventObserver>();

        return services;
    }
}