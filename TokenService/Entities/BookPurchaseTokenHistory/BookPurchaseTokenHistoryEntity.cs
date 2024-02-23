using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TokenService.Entities;

public enum BookPurchaseTokenHistoryType
{
    Add,
    Remove,
    OrderExecuted,
    OrderRefunded
}

public class BookPurchaseTokenHistoryEntity
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(200)]
    [ForeignKey(nameof(BookPurchaseToken))]
    public string UserId { get; set; }
    
    [MaxLength(200)] public string? OrderId { get; set; }
    
    public virtual BookPurchaseTokenEntity BookPurchaseToken { get; set; }

    public long Amount { get; set; }
    
    public long UpdatedBalance { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public BookPurchaseTokenHistoryType Type { get; set; }
}