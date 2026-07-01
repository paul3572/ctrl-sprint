using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class RouteResponse
{
    [JsonPropertyName("features")]
    public List<RouteFeature> Features { get; set; } = [];
}