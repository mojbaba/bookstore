using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using OrderService.CreateOrder;
using OrderService.Entities;

namespace OrderService.InventoryServiceEvents;

public class OrderedBooksPackingFailedEventHandler(
    IOrderRepository orderRepository,
    IEventPublishObservant eventPublishObservant)
{
    public async Task HandleAsync(OrderedBooksPackingFailedEvent @event, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(@event.OrderId, cancellationToken);
        order.Status = OrderStatus.Failed;
        order.FailReason = @event.Reason;
        await orderRepository.UpdateAsync(order, cancellationToken);

        await orderRepository.SaveChangesAsync(cancellationToken);

        var orderFailedEvent = new OrderFailedEvent()
        {
            OrderId = @event.OrderId,
            BookIds = @event.BookIds,
            TotalPrice = @event.TotalPrice,
            Reason = @event.Reason,
        };

        await eventPublishObservant.PublishAsync(orderFailedEvent);
    }
}