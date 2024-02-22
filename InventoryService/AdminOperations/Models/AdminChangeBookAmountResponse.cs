namespace InventoryService.AdminAddBook.Models;

public record AdminChangeBookAmountResponse
{
    public int UpdatedAmount { get; set; }
    public string BookId { get; set; }
}