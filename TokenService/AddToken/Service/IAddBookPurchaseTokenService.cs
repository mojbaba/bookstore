namespace TokenService.AddToken;

public interface IAddBookPurchaseTokenService
{
    Task<AddBookPurchaseTokenResponse> AddBookPurchaseTokenAsync(AddBookPurchaseTokenReqeust request, CancellationToken cancellationToken);
}