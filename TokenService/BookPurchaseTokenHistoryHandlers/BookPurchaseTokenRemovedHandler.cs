using TokenService.Entities;
using TokenService.RemoveToken;

namespace TokenService.BookPurchaseTokenHistoryHandlers;

public class BookPurchaseTokenRemovedHandler(IBookPurchaseTokenHistoryRepository historyRepository)
    : IBookPurchaseTokenRemovedHandler
{
    public async Task HandleBookPurchaseTokenRemovedAsync(BookPurchaseTokenRemovedEvent @event,
        CancellationToken cancellationToken)
    {
        var history = new BookPurchaseTokenHistoryEntity
        {
            UserId = @event.UserId,
            Amount = -1 * @event.RemovedAmount,
            UpdatedBalance = @event.UpdatedBalance,
            Type = BookPurchaseTokenHistoryType.Remove
        };

        await historyRepository.CreateAsync(history, cancellationToken);
        await historyRepository.SaveChangesAsync(cancellationToken);
    }
}