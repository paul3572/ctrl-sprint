using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute;

public class GeocodeGeometry
{
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = [];
}