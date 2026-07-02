using System.Net.Http.Json;
using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.domain.Exceptions;

namespace cts.core.svc.application.Services;

public sealed class OpenMeteoService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public OpenMeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherDto> GetWeatherAsync(double lat, double lon)
    {
        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(
            $"forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code");

        if (response == null)
            throw new WeatherNotFoundException("No weather data received.");

        Console.WriteLine("Weather Data:");
        Console.WriteLine(response.Current);
        
        return new WeatherDto(
            response.Current.Temperature,
            GetDescription(response.Current.WeatherCode));
    }

    private static string GetDescription(int weatherCode)
    {
        return weatherCode switch
        {
            0 => "Clear Sky",
            1 or 2 or 3 => "Partly Cloudy",
            45 or 48 => "Fog",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            71 or 73 or 75 => "Snow",
            95 => "Thunderstorm",
            _ => "Unknown"
        };
    }
}