namespace cts.core.svc.contracts.Tours;

public record TourCmd(
     Guid UserGuid,
     string Name,
     string Description,
     string From,
     string To,
     string TransportName,
     int TourDistanceKm,
     int EstimatedTimeMinutes,
     int Rating
);