namespace TokenService.AddToken;

public class AddBookPurchaseTokenException : Exception
{
    public AddBookPurchaseTokenException()
    {
    }

    public AddBookPurchaseTokenException(string message) : base(message)
    {
    }

    public AddBookPurchaseTokenException(string message, Exception inner) : base(message, inner)
    {
    }
}