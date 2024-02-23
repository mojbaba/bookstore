using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace InventoryService.KafkaOrderEventsConsumer;

public class OrderFailedEventObserver(IServiceProvider provider) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderFailedEvent orderFailedEvent)
        {
            return Task.CompletedTask;
        }

        var orderFailedEventService = provider.GetRequiredService<OrderFailedEventHandler>();
        return orderFailedEventService.HandleAsync(orderFailedEvent, CancellationToken.None);
    }
}