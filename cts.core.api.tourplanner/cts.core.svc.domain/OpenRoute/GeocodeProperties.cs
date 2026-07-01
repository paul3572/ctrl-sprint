using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class GeocodeProperties
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
    
    [JsonPropertyName("summary")]
    public GeocodeSummary GeocodeSummary { get; init; } = null!;
}