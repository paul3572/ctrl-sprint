using System.Net.Http.Json;
using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using Microsoft.Extensions.Configuration;

namespace cts.core.svc.application.Services;

public sealed class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"]!;
    }

    public async Task<WeatherDto> GetWeatherAsync(string city)
    {
        var response = await _httpClient.GetFromJsonAsync<WeatherDto>(
            $"weather?q={city}&appid={_apiKey}&units=metric");

        return response ?? throw new InvalidOperationException();
    }
}