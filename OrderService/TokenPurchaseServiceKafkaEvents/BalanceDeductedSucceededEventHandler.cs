using BookStore.Contracts;
using OrderService.Entities;

namespace OrderService.TokenPurchaseServiceKafkaEvents;

public class BalanceDeductedSucceededEventHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(BalanceDeductedSucceededEvent balanceDeductedSucceededEvent,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(balanceDeductedSucceededEvent.OrderId, cancellationToken);
        order.IsPaymentProcessed = true;

        if (order.IsInventoryProcessed)
        {
            order.Status = OrderStatus.Succeeded;
        }

        await orderRepository.UpdateAsync(order,cancellationToken);

        await orderRepository.SaveChangesAsync(cancellationToken);
    }
}