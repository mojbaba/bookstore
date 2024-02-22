namespace TokenService.RemoveToken;

public class RemoveBookPurchaseTokenResponse
{
    public string UserId { get; set; }
    public long RemovedAmount { get; set; }
    public long UpdatedBalance { get; set; }
}