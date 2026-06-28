using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class RouteFeature
{
    [JsonPropertyName("geometry")]
    public RouteGeometry RouteGeometry { get; set; } = null!;
    [JsonPropertyName("properties")]
    public RouteProperties Properties { get; set; } = null!;
}