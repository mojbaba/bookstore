using BookStore.EventObserver;
using TokenService.Entities;

namespace TokenService.AddToken;

public class AddBookPurchaseTokenService(
    IBookPurchaseTokenRepository tokenRepository,
    IEventPublishObservant eventPublishObservant) : IAddBookPurchaseTokenService
{
    public async Task<AddBookPurchaseTokenResponse> AddBookPurchaseTokenAsync(AddBookPurchaseTokenReqeust request,
        CancellationToken cancellationToken)
    {
        var existingToken = await tokenRepository.GetAsync(request.UserId, cancellationToken);
        
        if (existingToken != null)
        {
            existingToken.Amount += request.Amount;
            await tokenRepository.UpdateAsync(existingToken, cancellationToken);
        }
        else
        {
            var newToken = new BookPurchaseTokenEntity
            {
                UserId = request.UserId,
                Amount = request.Amount
            };
            await tokenRepository.CreateAsync(newToken, cancellationToken);
        }
        
        await tokenRepository.SaveChangesAsync(cancellationToken);
        
        await eventPublishObservant.PublishAsync(new BookPurchaseTokenAddedEvent
        {
            UserId = request.UserId,
            AddedAmount = request.Amount,
            UpdatedBalance = existingToken?.Amount ?? request.Amount
        });
        
        return new AddBookPurchaseTokenResponse
        {
            UserId = request.UserId,
            AddedAmount = request.Amount,
            UpdatedBalance = existingToken?.Amount ?? request.Amount
        };
    }
}