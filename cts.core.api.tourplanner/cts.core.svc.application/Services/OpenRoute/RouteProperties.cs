using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class RouteProperties
{
    [JsonPropertyName("summary")]
    public RouteSummary Summary { get; set; } = default!;
}