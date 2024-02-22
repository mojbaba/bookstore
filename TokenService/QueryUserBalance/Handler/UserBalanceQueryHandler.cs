using TokenService.Entities;

namespace TokenService.QueryUserBalance;

public class UserBalanceQueryHandler(IBookPurchaseTokenRepository tokenRepository) : IUserBalanceQueryHandler
{
    public async Task<QueryUserBalanceResponse> QueryUserBalanceAsync(QueryUserBalanceRequest request,
        CancellationToken cancellationToken)
    {
        var token = await tokenRepository.GetAsync(request.UserId, cancellationToken);
        var balance = token?.Amount ?? 0;
        
        return new QueryUserBalanceResponse
        {
            UserId = request.UserId,
            Balance = balance
        };
    }
}