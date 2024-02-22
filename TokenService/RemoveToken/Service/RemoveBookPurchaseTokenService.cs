using BookStore.EventObserver;
using TokenService.Entities;

namespace TokenService.RemoveToken;

public class RemoveBookPurchaseTokenService(IBookPurchaseTokenRepository repository, IEventPublishObservant eventPublishObservant) : IRemoveBookPurchaseTokenService
{
    public async Task<RemoveBookPurchaseTokenResponse> RemoveBookPurchaseTokenAsync(RemoveBookPurchaseTokenRequest request, CancellationToken cancellationToken)
    {
        var existingToken = await repository.GetAsync(request.UserId, cancellationToken);
        
        if (existingToken == null)
        {
            throw new RemoveBookPurchaseTokenException("Balance cannot be negative");
        }
        
        if (existingToken.Amount < request.Amount)
        {
            throw new RemoveBookPurchaseTokenException("Balance cannot be negative");
        }
        
        existingToken.Amount -= request.Amount;
        
        await repository.UpdateAsync(existingToken, cancellationToken);
        
        await repository.SaveChangesAsync(cancellationToken);
        
        await eventPublishObservant.PublishAsync(new BookPurchaseTokenRemovedEvent
        {
            UserId = request.UserId,
            RemovedAmount = request.Amount,
            UpdatedBalance = existingToken.Amount
        });
        
        return new RemoveBookPurchaseTokenResponse
        {
            UserId = request.UserId,
            RemovedAmount = request.Amount,
            UpdatedBalance = existingToken.Amount
        };
    }
}