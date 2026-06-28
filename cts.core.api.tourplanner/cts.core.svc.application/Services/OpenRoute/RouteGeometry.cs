using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class RouteGeometry
{
    [JsonPropertyName("coordinates")]
    public List<double[]> Coordinates { get; set; } = [];
}