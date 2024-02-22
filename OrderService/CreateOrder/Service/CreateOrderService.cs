using OrderService.Entities;

namespace OrderService.CreateOrder;

public class CreateOrderService(IOrderRepository repository) : ICreateOrderService
{
    public Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new OrderEntity
        {
            Status = OrderStatus.Processing
        };

        throw new NotImplementedException();
    }
}

