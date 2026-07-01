using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourLogRepository
{
    Task<List<TourLogDto>> GetToursLogsOfTour(Guid tourGuid);
    Task<TourLogDto?> GetTourLog(Guid tourLogGuid);
    Task<TourLogDto> CreateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<TourLogDto> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog);
    Task<TourLogDto> DeleteTourLog(Guid tourLogGuid);
}