using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class GeocodeSummary
{
    [JsonPropertyName("distance")]
    public double Distance { get; init; }

    [JsonPropertyName("duration")]
    public double Duration { get; init; }
}