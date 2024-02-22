using TokenService.AddToken;
using TokenService.Entities;

namespace TokenService.BookPurchaseTokenHistoryHandlers;

public class BookPurchaseTokenAddedHandler(IBookPurchaseTokenHistoryRepository historyRepository) : IBookPurchaseTokenAddedHandler
{
    public async Task HandleBookPurchaseTokenAddedAsync(BookPurchaseTokenAddedEvent @event, CancellationToken cancellationToken)
    {
        var history = new BookPurchaseTokenHistoryEntity
        {
            UserId = @event.UserId,
            Amount = @event.AddedAmount,
            UpdatedBalance = @event.UpdatedBalance,
            Type = BookPurchaseTokenHistoryType.Add
        };
        
        await historyRepository.CreateAsync(history, cancellationToken);
        await historyRepository.SaveChangesAsync(cancellationToken);
    }
}