namespace InventoryService.AdminAddBook.Models;

public record AdminAddBookRequest
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Amount { get; set; }
    public long Price { get; set; }
}