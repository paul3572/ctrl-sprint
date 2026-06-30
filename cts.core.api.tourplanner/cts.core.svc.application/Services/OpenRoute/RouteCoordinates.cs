namespace cts.core.svc.application.Services.OpenRoute;

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