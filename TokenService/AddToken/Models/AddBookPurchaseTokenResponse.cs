namespace TokenService.AddToken;

public record AddBookPurchaseTokenResponse
{
    public string UserId { get; set; }
    public long AddedAmount { get; set; }
    
    public long UpdatedBalance { get; set; }
}