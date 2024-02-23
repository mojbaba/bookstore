using BookStore.Contracts;
using BookStore.EventObserver;
using OrderService.InventoryServiceEvents;

namespace OrderService.InventoryServiceKafkaEvents;

public class OrderedBooksPackingFailedEventObserver(IServiceProvider provider) : IEventPublishObserver
{
    public Task OnEventPublished(EventBase @event)
    {
        if(@event is not OrderedBooksPackingFailedEvent orderedBooksPackingFailedEvent)
        {
            return Task.CompletedTask;
        }
        
        var handler = provider.GetRequiredService<OrderedBooksPackingFailedEventHandler>();
        return handler.HandleAsync(orderedBooksPackingFailedEvent, CancellationToken.None);
    }
}