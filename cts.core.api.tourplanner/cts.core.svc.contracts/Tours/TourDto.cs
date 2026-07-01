using cts.core.svc.contracts.TourLogs;
using cts.core.svc.domain.OpenRoute;

namespace cts.core.svc.contracts.Tours;

public record TourDto(
    Guid TourGuid,
    Guid UserGuid,
    string Name,
    string Description,
    string From,
    string To,
    string TransportName,
    double TourDistanceInMeters,
    int EstimatedTimeMinutes,
    int Rating,
    List<TourLogDto> TourLogs,
    RouteGeometry? RouteGeometry
);