namespace InventoryService.AdminAddBook.Models;

public record AdminChangeBookAmountRequest
{
    public string BookId { get; set; }
    public int Amount { get; set; }
}