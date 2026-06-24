using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourLogService
{
    Task<ActionResult<List<TourLogDto>>> GetTourLogsOfTour(Guid userGuid);
    Task<ActionResult<TourLogDto>> GetTourLog(Guid tourGuid);
    Task<ActionResult<Guid>> CreateTourLog(Guid userGuid, TourLogCmd tour);
    Task<ActionResult<Guid>> UpdateTourLog(Guid tourGuid, TourLogCmd tour);
    Task<ActionResult<string>> DeleteTourLog(Guid tourGuid);
}