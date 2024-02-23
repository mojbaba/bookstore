using OrderService.CreateOrder;

namespace InventoryService.KafkaOrderEventsConsumer;

public class OrderFailedEventHandler
{
    public Task HandleAsync(OrderFailedEvent orderFailedEvent, CancellationToken cancellationToken)
    {
        // this must be implemented to undo the order execution, 
        // for example, if the order was created, the stock must be returned to the inventory
        return Task.CompletedTask;
    }
}