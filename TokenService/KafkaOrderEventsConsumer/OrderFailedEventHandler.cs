using BookStore.RedisLock;
using OrderService.CreateOrder;
using TokenService.Entities;

namespace TokenService.KafkaOrderEventsConsumer;

public class OrderFailedEventHandler(
    IBookPurchaseTokenRepository tokenRepository,
    IBookPurchaseTokenHistoryRepository historyRepository,
    IDistributedLock distributedLock)
{
    public async Task HandleAsync(OrderFailedEvent orderFailedEvent, CancellationToken cancellationToken)
    {
        var tokenHistories =
            await historyRepository.GetItemsByOrderIdAsync(orderFailedEvent.OrderId, cancellationToken);

        // If the order is refunded, we don't need to do anything
        var isRefunded = tokenHistories.Any(x => x.Type == BookPurchaseTokenHistoryType.OrderRefunded);
        var hasProcessedOrderId = tokenHistories.Any();
        
        if (isRefunded || !hasProcessedOrderId)
        {
            return;
        }
        
        

        // If the order is failed, we need to refund the tokens
        var lockKey = await distributedLock.WaitToAcquireLockAsync(orderFailedEvent.OrderId, TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10));
        
        var token = await tokenRepository.GetAsync(orderFailedEvent.OrderId, cancellationToken);

        token.Amount += orderFailedEvent.TotalPrice;
        
        // better to have some unit of work pattern here, transaction
        
        await tokenRepository.UpdateAsync(token, cancellationToken);

        await tokenRepository.SaveChangesAsync(cancellationToken);
        
        await historyRepository.CreateAsync(new BookPurchaseTokenHistoryEntity
        {
            OrderId = orderFailedEvent.OrderId,
            Amount = orderFailedEvent.TotalPrice,
            Type = BookPurchaseTokenHistoryType.OrderRefunded,
            CreatedAt = DateTime.UtcNow,
            UserId = token.UserId,
            UpdatedBalance = token.Amount
        }, cancellationToken);

        await historyRepository.SaveChangesAsync(cancellationToken);
    }
}