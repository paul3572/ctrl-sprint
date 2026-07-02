namespace cts.core.svc.domain.Exceptions;

public class WeatherNotFoundException : Exception
{
    public WeatherNotFoundException()
    {
    }

    public WeatherNotFoundException(string? message) : base(message)
    {
    }

    public WeatherNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}