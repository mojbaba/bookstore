namespace InventoryService.QueryBooks;

public record QueryBookRequest
{
    public IEnumerable<string> BookIds { get; set; } = new string[]{};
    public string AuthenticationToken { get; set; }
}