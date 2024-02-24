using Microsoft.AspNetCore.Mvc;
using OrderService.Entities;

namespace OrderService.Admin;

[ApiController]
[Route("api/admin")]
public class AdminController(IOrderRepository orderRepository) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<IEnumerable<OrderEntity>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        return await orderRepository.AllAsync(cancellationToken);
    }
}