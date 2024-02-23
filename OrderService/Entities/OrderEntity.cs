using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public enum OrderStatus
{
    Processing,
    Succeeded,
    Failed
}

public class OrderEntity
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(200)] public string UserId { get; set; }
    
    public OrderStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();

    public bool IsPaymentProcessed { get; set; }

    public bool IsInventoryProcessed { get; set; }
    
    public string? FailReason { get; set; }
}

public class OrderItemEntity
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(200)] public string BookId { get; set; }
    
    public long Price { get; set; }
}