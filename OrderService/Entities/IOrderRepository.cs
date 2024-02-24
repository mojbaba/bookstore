using BookStore.Repository;

namespace OrderService.Entities;

public interface IOrderRepository : IRepository<OrderEntity>
{
    public Task<IEnumerable<OrderEntity>> AllAsync(CancellationToken cancellationToken);
}