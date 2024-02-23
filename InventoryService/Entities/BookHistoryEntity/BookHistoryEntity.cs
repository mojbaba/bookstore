namespace InventoryService.Entities;

public class BookHistoryEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string BookId { get; set; }

    public string? OrderId { get; set; }

    public int Amount { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int UpdatedAmount { get; set; }

    public BookHistoryType Type { get; set; }
}

public enum BookHistoryType
{
    OrderExecuted,
    OrderCancelled
}