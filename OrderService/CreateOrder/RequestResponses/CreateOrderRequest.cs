namespace OrderService.CreateOrder;

public record CreateOrderRequest
{
    public IEnumerable<string> BookIds { get; set; }
    public string AuthenticationToken { get; set; }
    public string UserId { get; set; }
}