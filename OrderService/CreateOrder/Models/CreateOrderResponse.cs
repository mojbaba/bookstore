using OrderService.Entities;

namespace OrderService.CreateOrder;

public record CreateOrderResponse
{
    public string OrderId { get; set; }
    public IEnumerable<string> BookIds { get; set; }
}