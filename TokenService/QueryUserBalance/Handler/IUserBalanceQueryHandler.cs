namespace TokenService.QueryUserBalance;

public interface IUserBalanceQueryHandler
{
    Task<QueryUserBalanceResponse> QueryUserBalanceAsync(QueryUserBalanceRequest request, CancellationToken cancellationToken);
}