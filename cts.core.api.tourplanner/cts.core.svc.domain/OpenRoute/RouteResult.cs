namespace cts.core.svc.domain.OpenRoute;

public record RouteResult(
    double DistanceInMeters,
    int EstimatedTimeMin,
    RouteGeometry Geometry
);