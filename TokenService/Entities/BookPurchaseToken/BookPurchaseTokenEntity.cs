using System.ComponentModel.DataAnnotations;

namespace TokenService.Entities;

public class BookPurchaseTokenEntity
{
    [Key] [MaxLength(200)] public string UserId { get; set; }

    [Range(0, long.MaxValue)] public long Amount { get; set; }
    
    public ICollection<BookPurchaseTokenHistoryEntity> History { get; set; } = new List<BookPurchaseTokenHistoryEntity>();
}