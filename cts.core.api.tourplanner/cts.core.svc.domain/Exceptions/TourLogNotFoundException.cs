namespace cts.core.svc.domain.Exceptions;

public class TourLogNotFoundException : Exception
{
    public TourLogNotFoundException()
    {
    }

    public TourLogNotFoundException(string? message) : base(message)
    {
    }

    public TourLogNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}