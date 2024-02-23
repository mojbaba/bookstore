using BookStore.Contracts;
using BookStore.EventObserver;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class BalanceDeductedSucceededEventObserver(IServiceProvider provider) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not BalanceDeductedSucceededEvent balanceDeductedSucceededEvent)
        {
            return Task.CompletedTask;
        }

        var handler = provider.GetRequiredService<BalanceDeductedSucceededEventHandler>();
        return handler.HandleAsync(balanceDeductedSucceededEvent, CancellationToken.None);
    }
}