using BookStore.Contracts;
using BookStore.EventObserver;
using OrderService.CreateOrder;
using OrderService.Entities;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class BalanceDeductionFailedEventHandler(
    IEventPublishObservant eventPublishObservant,
    IOrderRepository orderRepository)
{
    public async Task HandleAsync(BalanceDeductionFailedEvent balanceDeductionFailedEvent,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(balanceDeductionFailedEvent.OrderId, cancellationToken);
        var previousStatus = order.Status;

        order.Status = OrderStatus.Failed;

        if (order.FailReason is not null)
        {
            order.FailReason += $", {balanceDeductionFailedEvent.Reason}";
        }
        else
        {
            order.FailReason = balanceDeductionFailedEvent.Reason;
        }

        await orderRepository.UpdateAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        if(previousStatus == OrderStatus.Failed) //already published event
        {
            return;
        }
        
        await eventPublishObservant.PublishAsync(new OrderFailedEvent
        {
            OrderId = balanceDeductionFailedEvent.OrderId,
            BookIds = balanceDeductionFailedEvent.BookIds,
            TotalPrice = balanceDeductionFailedEvent.TotalPrice,
            Reason = balanceDeductionFailedEvent.Reason
        });
    }
}