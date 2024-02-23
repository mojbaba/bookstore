using BookStore.Contracts;
using BookStore.EventObserver;
using OrderService.InventoryServiceEvents;

namespace OrderService.InventoryServiceKafkaEvents;

public class OrderedBooksPackedEventObserver(IServiceProvider provider) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if (@event is not OrderedBooksPackedEvent orderedBooksPackedEvent)
        {
            return Task.CompletedTask;
        }

        var handler = provider.GetRequiredService<OrderedBooksPackedEventHandler>();
        return handler.HandleAsync(orderedBooksPackedEvent, CancellationToken.None);
    }
}