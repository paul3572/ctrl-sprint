using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace TourGuideApplication.Interfaces;

public interface ITourLogController
{
    Task<ActionResult<List<TourDto>>> GetToursLogsOfTour(Guid tourGuid);
    Task<ActionResult<TourDto>> GetTourLog(Guid tourLogGuid);
    Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog);
    Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourGuid, TourLogCmd tour);
    Task<ActionResult<string>> DeleteTourLog(Guid tourGuid);
}