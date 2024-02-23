using BookStore.EventObserver;
using OrderService.CreateOrder;

namespace InventoryService.KafkaOrderEventsConsumer;

public class OrderCreatedEventObserver(OrderCreatedEventHandler handler) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderCreatedEvent orderCreatedEvent)
        {
            return Task.CompletedTask;
        }

        return handler.HandleAsync(orderCreatedEvent, CancellationToken.None);
    }
}

