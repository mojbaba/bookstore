namespace InventoryService.QueryBooks;

public record Book
{
    public string BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public long Price { get; set; }
}
public record QueryBookResponse
{
    public IEnumerable<Book> Books { get; set; }
}