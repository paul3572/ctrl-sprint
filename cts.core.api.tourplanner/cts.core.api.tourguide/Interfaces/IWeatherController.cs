using cts.core.svc.contracts;

namespace TourGuideApplication.Interfaces;

public interface IWeatherController
{
    Task<WeatherDto> GetWeather(double lat, double lon);
}