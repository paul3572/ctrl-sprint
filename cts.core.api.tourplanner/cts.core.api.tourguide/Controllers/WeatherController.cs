using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase, IWeatherController
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        this._weatherService = weatherService;
        this._logger = logger;
    }

    [HttpGet]
    public async Task<WeatherDto> GetWeather([FromQuery] double lat, [FromQuery] double lon)
    {
        try
        {
            return await _weatherService.GetWeatherAsync(lat, lon);
        }
        catch (WeatherNotFoundException ex)
        {
            this._logger.LogWarning(ex, "Weather not found.");
            
            return new WeatherDto(27, string.Empty);
        }
    }
}