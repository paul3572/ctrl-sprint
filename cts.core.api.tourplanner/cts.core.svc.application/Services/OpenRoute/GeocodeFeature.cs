using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class GeocodeFeature
{
    [JsonPropertyName("geometry")]
    public GeocodeGeometry Geometry { get; set; } = null!;
    [JsonPropertyName("properties")]
    public GeocodeProperties GeocodeProperties { get; set; } = null!;
}