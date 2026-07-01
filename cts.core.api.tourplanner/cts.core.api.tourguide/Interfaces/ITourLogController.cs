using cts.core.svc.contracts.TourLogs;
using Microsoft.AspNetCore.Mvc;

namespace TourGuideApplication.Interfaces;

public interface ITourLogController
{
    Task<ActionResult<List<TourLogDto>>> GetToursLogsOfTour(Guid tourGuid);
    Task<ActionResult<TourLogDto>> GetTourLog(Guid tourLogGuid);
    Task<ActionResult<TourLogDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<ActionResult<TourLogDto>> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog);
    Task<ActionResult<TourLogDto>> DeleteTourLog(Guid tourLogGuid);
}