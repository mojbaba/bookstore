using Microsoft.EntityFrameworkCore;

namespace OrderService.Entities;

public class OrderRepository(OrderServiceDbContext dbContext) : IOrderRepository
{
    public Task<OrderEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Orders.FirstOrDefaultAsync(a => a.Id == id.ToString(), cancellationToken);
    }

    public Task<OrderEntity?> GetAsync(string id, CancellationToken cancellationToken)
    {
        return dbContext.Orders.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<OrderEntity> CreateAsync(OrderEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Orders.Add(entity).Entity);
    }

    public Task<OrderEntity> UpdateAsync(OrderEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.Orders.Update(entity).Entity);
    }

    public Task<OrderEntity> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = dbContext.Orders.First(a => a.Id == id.ToString());
        if(entity == null)
        {
            throw new InvalidOperationException("Entity not found");
        }
        
        return Task.FromResult(dbContext.Orders.Remove(entity).Entity);
    }

    public Task<OrderEntity> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = dbContext.Orders.First(a => a.Id == id);
        if(entity == null)
        {
            throw new InvalidOperationException("Entity not found");
        }
        
        return Task.FromResult(dbContext.Orders.Remove(entity).Entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}