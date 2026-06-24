using System.Net.Http.Json;
using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;

namespace cts.core.svc.application.Services;

public sealed class OpenMeteoService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public OpenMeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherDto> GetWeatherAsync(string city)
    {
        var (lat, lon) = city.ToLower() switch
        {
            "wien" => (48.2082, 16.3738),
            "linz" => (48.3069, 14.2858),
            "salzburg" => (47.8095, 13.0550),
            "boston" => (42.3584, -71.0598),
            _ => throw new ArgumentException($"Unknown city: {city}")
        };

        var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(
            $"forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code");

        if (response == null)
            throw new InvalidOperationException("No weather data received.");

        Console.WriteLine("Weather Data:");
        Console.WriteLine(response.Current);
        
        return new WeatherDto(
            city,
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