namespace TokenService.RemoveToken;

public interface IRemoveBookPurchaseTokenService
{
    Task<RemoveBookPurchaseTokenResponse> RemoveBookPurchaseTokenAsync(RemoveBookPurchaseTokenRequest request, CancellationToken cancellationToken);
}