namespace cts.core.svc.domain.Exceptions;

public class TourException : Exception
{
    public TourException()
    {
    }

    public TourException(string? message) : base(message)
    {
    }

    public TourException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}