using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class RouteRequest
{
    [JsonPropertyName("coordinates")]
    public required List<RouteCoordinates> Coordinates { get; set; }
}