using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using Microsoft.AspNetCore.Mvc;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}")]
    public async Task<WeatherDto> GetWeather(string city)
    {
        return await _weatherService.GetWeatherAsync(city);
    }
}