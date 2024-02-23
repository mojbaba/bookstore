namespace OrderService.TokenPurchaseServiceKafkaEvents;

public static class ServiceRegisteration
{
    public static IServiceCollection AddTokenPurchaseServiceKafkaEvents(this IServiceCollection services)
    {
        services.AddTransient<BalanceDeductedSucceededEventHandler>();
        services.AddTransient<BalanceDeductionFailedEventHandler>();

        services.AddHostedService<KafkaBalanceDeductedSucceededEventConsumer>();
        services.AddHostedService<KafkaBalanceDeductionFailedEventConsumer>();
        
        return services;
    }
}