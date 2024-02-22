namespace TokenService.RemoveToken;

public class RemoveBookPurchaseTokenException : Exception
{
    public RemoveBookPurchaseTokenException()
    {
    }

    public RemoveBookPurchaseTokenException(string message) : base(message)
    {
    }

    public RemoveBookPurchaseTokenException(string message, Exception inner) : base(message, inner)
    {
    }
}