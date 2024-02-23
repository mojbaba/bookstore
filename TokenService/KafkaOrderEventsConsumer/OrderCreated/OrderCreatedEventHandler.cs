using BookStore.Contracts;
using BookStore.EventLog.Kafka;
using BookStore.RedisLock;
using OrderService.CreateOrder;
using TokenService.Entities;

namespace TokenService.KafkaOrderEventsConsumer;

public class OrderCreatedEventHandler(
    IBookPurchaseTokenRepository bookPurchaseTokenRepository,
    IBookPurchaseTokenHistoryRepository purchaseTokenHistoryRepository,
    IEventLogProducer eventLogProducer,
    KafkaOptions kafkaOptions,
    IDistributedLock distributedLock)
{
    public async Task HandleAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken)
    {
        //check if the order is already processed
        var bookPurchaseToken =
            await purchaseTokenHistoryRepository.GetItemsByOrderIdAsync(orderCreatedEvent.OrderId, cancellationToken);

        if (bookPurchaseToken.Any())
            return; //do nothing

        var lockId = await distributedLock.WaitToAcquireLockAsync(orderCreatedEvent.UserId, TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10));
        //better to add a transaction here to make sure that the token is added and the history is added

        try
        {
            var userTokenBalance =
                await bookPurchaseTokenRepository.GetAsync(orderCreatedEvent.UserId, cancellationToken);

            var userBalance = userTokenBalance?.Amount ?? 0;

            if (userBalance < orderCreatedEvent.TotalPrice)
            {
                var balandeDeductionFailedEvent = new BalanceDeductionFailedEvent()
                {
                    OrderId = orderCreatedEvent.OrderId,
                    BookIds = orderCreatedEvent.BookIds,
                    TotalPrice = orderCreatedEvent.TotalPrice,
                    Reason = $"User balance is not enough to purchase the books. User balance: {userBalance}"
                };

                await SendBalanceDeductionFailedEvent(balandeDeductionFailedEvent, cancellationToken);
                return;
            }

            userTokenBalance.Amount = userTokenBalance.Amount - orderCreatedEvent.TotalPrice;

            await bookPurchaseTokenRepository.UpdateAsync(userTokenBalance, cancellationToken);

            await bookPurchaseTokenRepository.SaveChangesAsync(cancellationToken);

            var bookPurchaseTokenHistory = new BookPurchaseTokenHistoryEntity()
            {
                OrderId = orderCreatedEvent.OrderId,
                UserId = orderCreatedEvent.UserId,
                Amount = -1 * orderCreatedEvent.TotalPrice,

                CreatedAt = DateTime.UtcNow
            };

            await purchaseTokenHistoryRepository.CreateAsync(bookPurchaseTokenHistory, cancellationToken);
            await purchaseTokenHistoryRepository.SaveChangesAsync(cancellationToken);

            await SendBalanceDeductedSecceededEvent(new BalanceDeductedSucceededEvent()
            {
                OrderId = orderCreatedEvent.OrderId,
                TotalPrice = orderCreatedEvent.TotalPrice,
                BookIds = orderCreatedEvent.BookIds
            }, cancellationToken);
        }
        finally
        {
            distributedLock.TryReleaseLock(orderCreatedEvent.UserId, lockId);
        }
    }

    private Task SendBalanceDeductionFailedEvent(BalanceDeductionFailedEvent balanceDeductionFailedEvent,
        CancellationToken cancellationToken)
    {
        //send an event to the order service to cancel the order
        return eventLogProducer.ProduceAsync(kafkaOptions.Topics.BalanceDeductionFailedTopic,
            balanceDeductionFailedEvent,
            cancellationToken);
    }

    private Task SendBalanceDeductedSecceededEvent(BalanceDeductedSucceededEvent balanceDeductedSucceededEvent,
        CancellationToken cancellationToken)
    {
        //send an event to the order service to tell the payment is processed
        return eventLogProducer.ProduceAsync(kafkaOptions.Topics.BalanceDeductedTopic,
            balanceDeductedSucceededEvent,
            cancellationToken);
    }
}