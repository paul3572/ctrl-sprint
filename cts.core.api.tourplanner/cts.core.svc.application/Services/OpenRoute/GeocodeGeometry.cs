using System.Text.Json.Serialization;

namespace cts.core.svc.application.Services.OpenRoute;

public class GeocodeGeometry
{
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = [];
}