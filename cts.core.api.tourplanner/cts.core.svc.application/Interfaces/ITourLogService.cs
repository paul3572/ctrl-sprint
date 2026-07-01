using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourLogService
{
    Task<List<TourLogDto>> GetTourLogsOfTour(Guid tourGuid);
    Task<TourLogDto> GetTourLog(Guid tourLogGuid);
    Task<TourLogDto> CreateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<TourLogDto> UpdateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<TourLogDto> DeleteTourLog(Guid tourGuid);
}