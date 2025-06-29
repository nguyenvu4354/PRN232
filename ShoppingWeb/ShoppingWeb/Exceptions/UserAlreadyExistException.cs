namespace ShoppingWeb.Exceptions;

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException(string message) : base(message)
    {
    }
    public UserAlreadyExistException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
