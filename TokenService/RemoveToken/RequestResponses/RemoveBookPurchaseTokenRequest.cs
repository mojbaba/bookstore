namespace TokenService.RemoveToken;

public class RemoveBookPurchaseTokenRequest
{
    public string UserId { get; set; }
    public long Amount { get; set; }
}