using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class RouteResponse
{
    [JsonPropertyName("features")]
    public List<RouteFeature> Features { get; set; } = [];
}