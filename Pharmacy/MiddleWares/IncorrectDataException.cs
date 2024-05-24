namespace Pharmacy_.MiddleWares;

public class IncorrectDataException : Exception
{
    public IncorrectDataException(string? message) : base(message)
    {
    }
}