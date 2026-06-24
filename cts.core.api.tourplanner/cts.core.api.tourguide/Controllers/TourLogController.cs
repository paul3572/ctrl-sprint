using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourLogController : ControllerBase, ITourLogController
{
    private readonly ITourLogService tourLogService;

    public TourLogController(ITourLogService tourLogService)
    {
        this.tourLogService = tourLogService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TourDto>>> GetToursLogsOfTour(Guid tourGuid)
    {
        try
        {
            return Ok(await this.tourLogService.GetTourLogsOfTour(tourGuid));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpGet("{tourLogGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourDto>> GetTourLog(Guid tourLogGuid)
    {
        try
        {
            return Ok(await this.tourLogService.GetTourLog(tourLogGuid));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPost("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        try
        {
            return Ok(await this.tourLogService.CreateTourLog(tourGuid, tourLog));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPatch("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog)
    {
        try
        {
            return Ok(await this.tourLogService.UpdateTourLog(tourLogGuid, tourLog));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpDelete("{tourLogGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuidDto>> DeleteTourLog(Guid tourLogGuid)
    {
        try
        {
            return Ok(await this.tourLogService.DeleteTourLog(tourLogGuid));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }
}