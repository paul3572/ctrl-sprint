namespace cts.core.svc.contracts.TourLogs;

public record TourLogCmd(
    Guid TourGuid,
    DateTime Timestamp,
    string Comment,
    int Difficulty,
    int TotalDistanceKm,
    int TotalTimeMin,
    int Rating
);