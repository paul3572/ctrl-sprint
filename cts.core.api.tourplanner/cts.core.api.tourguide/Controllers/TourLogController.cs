using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

public class TourLogController : ControllerBase, ITourLogController
{
    private readonly ITourLogService tourLogService;

    public TourLogController(ITourLogService tourLogService)
    {
        this.tourLogService = tourLogService;
    }
    
    public Task<ActionResult<List<TourDto>>> GetToursLogsOfTour(Guid tourGuid)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<TourDto>> GetTourLog(Guid tourLogGuid)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourGuid, TourLogCmd tour)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<string>> DeleteTourLog(Guid tourGuid)
    {
        throw new NotImplementedException();
    }
}