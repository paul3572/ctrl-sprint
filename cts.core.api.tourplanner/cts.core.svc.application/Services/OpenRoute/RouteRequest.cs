using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class RouteRequest
{
    [JsonPropertyName("coordinates")]
    public required List<RouteCoordinates> Coordinates { get; set; }
}