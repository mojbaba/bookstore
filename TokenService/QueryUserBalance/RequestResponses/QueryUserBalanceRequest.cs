namespace TokenService.QueryUserBalance;

public record QueryUserBalanceRequest
{
    public string UserId { get; set; }
}