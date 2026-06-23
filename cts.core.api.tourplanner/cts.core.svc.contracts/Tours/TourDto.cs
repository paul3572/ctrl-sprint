using cts.core.svc.contracts.TourLogs;

namespace cts.core.svc.contracts.Tours;

public record TourDto(
    Guid TourGuid,
    Guid UserGuid,
    string Name,
    string Description,
    string From,
    string To,
    string TransportName,
    int TourDistanceKm,
    int EstimatedTimeMinutes,
    int Rating,
    List<TourLogDto> TourLogs
);