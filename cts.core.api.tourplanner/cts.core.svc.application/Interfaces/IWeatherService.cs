using cts.core.svc.contracts;

namespace cts.core.svc.application.Interfaces;

public interface IWeatherService
{
    Task<WeatherDto> GetWeatherAsync(double lat, double lon);
}