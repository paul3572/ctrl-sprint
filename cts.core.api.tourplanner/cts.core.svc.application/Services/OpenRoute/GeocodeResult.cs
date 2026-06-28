namespace cts.core.svc.application.Services.OpenRoute;

public record GeocodeResult(
    double Latitude,
    double Longitude,
    string Label
);