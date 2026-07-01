using cts.core.svc.application.Services;
using Microsoft.Extensions.Configuration;

namespace cts.core.tests.tourguide;

public class OpenRouteServiceTests
{
    private OpenRouteService service;

    [SetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<OpenRouteServiceTests>()
            .Build();

        var apiKey =
            config["OpenRouteService:ApiKey"] ?? throw new InvalidOperationException("OpenRouteService API key is not configured.");

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openrouteservice.org/")
        };

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

        this.service = new OpenRouteService(httpClient);
    }
    
    [Test]
    [Explicit("Requires OpenRouteService API key")]
    public async Task GeocodeAsync_ShouldReturnRealCoordinates_ForVienna()
    {
        var result = await service.GeocodeAsync("Vienna");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Latitude, Is.InRange(48, 49));
        Assert.That(result.Longitude, Is.InRange(16, 17));
    }
    
    [Test]
    [Explicit("Requires OpenRouteService API key")]
    public async Task GetRouteAsync_ShouldReturnRealRoute()
    {
        var result = await service.GetRouteAsync(
            "Vienna, Austria",
            "Budapest, Hungary",
            "driving-car");

        Assert.That(result.DistanceInMeters, Is.GreaterThan(10000));
        Assert.That(result.EstimatedTimeMin, Is.GreaterThan(60));
        Assert.That(result.Geometry.Coordinates.Count, Is.GreaterThan(1));
    }
}