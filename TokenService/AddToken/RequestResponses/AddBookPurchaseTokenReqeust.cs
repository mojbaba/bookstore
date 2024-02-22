namespace TokenService.AddToken;

public record AddBookPurchaseTokenReqeust
{
    public string UserId { get; set; }
    public long Amount { get; set; }
}