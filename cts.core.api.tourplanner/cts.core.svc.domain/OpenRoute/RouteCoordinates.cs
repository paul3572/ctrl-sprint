using System.Text.Json.Serialization;
using cts.core.svc.domain.OpenRoute.Converter;

namespace cts.core.svc.domain.OpenRoute;

[JsonConverter(typeof(RouteCoordinatesJsonConverter))]
public class RouteCoordinates
{
    public RouteCoordinates(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }

    public double Longitude { get; set; } = 0.0;
    public double Latitude { get; set; } = 0.0;

    public override string ToString()
    {
        return $"{Longitude},{Latitude}";
    }
}