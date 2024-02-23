using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class BalanceDeductionFailedEventObserver(IServiceProvider serviceProvider) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not BalanceDeductionFailedEvent balanceDeductionFailedEvent)
        {
            return Task.CompletedTask;
        }

        var handler = serviceProvider.GetRequiredService<BalanceDeductionFailedEventHandler>();
        return handler.HandleAsync(balanceDeductionFailedEvent, CancellationToken.None);
    }
}