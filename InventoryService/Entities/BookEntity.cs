using System.ComponentModel.DataAnnotations;

namespace InventoryService.Entities;

public class BookEntity
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Author { get; set; }
    public int Amount { get; set; }
    public long Price { get; set; }
}