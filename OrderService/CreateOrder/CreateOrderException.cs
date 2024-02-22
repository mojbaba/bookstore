namespace OrderService.CreateOrder;

public class CreateOrderException : Exception
{
    public CreateOrderException()
    {
    }

    public CreateOrderException(string message) : base(message)
    {
    }

    public CreateOrderException(string message, Exception inner) : base(message, inner)
    {
    }
}