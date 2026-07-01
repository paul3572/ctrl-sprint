using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class RouteGeometry
{
    [JsonPropertyName("coordinates")]
    public List<RouteCoordinates> Coordinates { get; set; } = [];
}