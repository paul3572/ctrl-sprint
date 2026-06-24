namespace cts.core.svc.contracts;

public record WeatherDto(
    string City,
    double Temperature,
    string Description
);