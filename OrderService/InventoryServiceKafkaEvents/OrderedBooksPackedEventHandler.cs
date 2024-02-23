using BookStore.Contracts;
using BookStore.EventObserver;
using BookStore.RedisLock;
using OrderService.Entities;

namespace OrderService.InventoryServiceEvents;

public class OrderedBooksPackedEventHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(OrderedBooksPackedEvent @event,CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(@event.OrderId, cancellationToken);
        if (order == null)
        {
            return;
        }

        order.IsInventoryProcessed = true;
        
        if(order.IsPaymentProcessed)
        {
            order.Status = OrderStatus.Succeeded;
        }
        
        await orderRepository.UpdateAsync(order, cancellationToken);
    }
}