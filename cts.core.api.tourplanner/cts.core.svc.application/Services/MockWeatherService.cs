using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;

namespace cts.core.svc.application.Services;

public class MockWeatherService : IWeatherService
{
    public Task<WeatherDto> GetWeatherAsync(string city)
    {
        WeatherDto weather = city.ToLower() switch
        {
            "linz" => new WeatherDto(
                "Linz",
                24.5,
                "Sunny"),

            "wien" => new WeatherDto(
                "Wien",
                22.1,
                "Cloudy"),

            _ => new WeatherDto(
                city,
                20.0,
                "Unknown")
        };

        return Task.FromResult(weather);
    }
}