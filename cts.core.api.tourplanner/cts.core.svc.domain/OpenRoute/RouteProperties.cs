using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class RouteProperties
{
    [JsonPropertyName("summary")]
    public RouteSummary Summary { get; set; } = default!;
}