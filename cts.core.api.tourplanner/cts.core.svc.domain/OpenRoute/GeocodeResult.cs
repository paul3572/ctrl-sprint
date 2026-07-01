namespace cts.core.svc.domain.OpenRoute;

public record GeocodeResult(
    double Latitude,
    double Longitude,
    string Label
);