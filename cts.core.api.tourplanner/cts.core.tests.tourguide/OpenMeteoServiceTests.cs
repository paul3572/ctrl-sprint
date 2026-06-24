using cts.core.svc.application.Services;

namespace cts.core.tests.tourguide;

public class OpenMeteoServiceTests
{
    [Test]
    public async Task GetWeatherAsync_ShouldReturnWeatherForVienna()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.open-meteo.com/v1/")
        };

        var service = new OpenMeteoService(httpClient);

        // Act
        var result = await service.GetWeatherAsync(48.210033, 16.363449);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.Not.Empty);
        Assert.That(result.Temperature, Is.InRange(-50, 60));
    }
}