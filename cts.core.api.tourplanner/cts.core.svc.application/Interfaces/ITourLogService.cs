using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourLogService
{
    Task<ActionResult<List<TourLogDto>>> GetTourLogsOfTour(Guid tourGuid);
    Task<ActionResult<TourLogDto>> GetTourLog(Guid tourLogGuid);
    Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<ActionResult<GuidDto>> DeleteTourLog(Guid tourGuid);
}