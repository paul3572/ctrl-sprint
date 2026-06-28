namespace cts.core.svc.application.Services.OpenRoute;

public record RouteResult(
    double DistanceInMeters,
    int EstimatedTimeMin,
    List<double[]> Geometry
);