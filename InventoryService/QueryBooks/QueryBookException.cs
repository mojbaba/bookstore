namespace InventoryService.QueryBooks;

public class QueryBookException : Exception
{
    public QueryBookException()
    {
    }

    public QueryBookException(string message) : base(message)
    {
    }

    public QueryBookException(string message, Exception inner) : base(message, inner)
    {
    }
}