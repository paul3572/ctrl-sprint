using System.Text.Json.Serialization;

namespace cts.core.svc.contracts;

public sealed class OpenMeteoResponse
{
    public CurrentWeather Current { get; set; } = default!;
}

public sealed class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}