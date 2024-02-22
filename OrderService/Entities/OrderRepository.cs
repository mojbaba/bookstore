namespace OrderService.Entities;

public class OrderRepository : IOrderRepository
{
    public Task<OrderEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntity> CreateAsync(OrderEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntity> UpdateAsync(OrderEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<OrderEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}