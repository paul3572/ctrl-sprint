using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase, IWeatherController
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<WeatherDto> GetWeather([FromQuery] double lat, [FromQuery] double lon)
    {
        return await _weatherService.GetWeatherAsync(lat, lon);
    }
}