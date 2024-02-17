namespace UserService.Register;

public class UserRegisterException : Exception
{
    public UserRegisterException()
    {
    }

    public UserRegisterException(string message) : base(message)
    {
    }

    public UserRegisterException(string message, Exception inner) : base(message, inner)
    {
    }
}