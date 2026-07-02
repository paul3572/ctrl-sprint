using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using cts.core.svc.domain.Exceptions;
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
    public async Task<ActionResult<List<TourLogDto>>> GetToursLogsOfTour(Guid tourGuid)
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
    public async Task<ActionResult<TourLogDto>> GetTourLog(Guid tourLogGuid)
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourLogDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        try
        {
            return Ok(await this.tourLogService.CreateTourLog(tourGuid, tourLog));
        }
        catch (TourNotFoundException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPatch("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourLogDto>> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog)
    {
        try
        {
            return Ok(await this.tourLogService.UpdateTourLog(tourLogGuid, tourLog));
        }
        catch (TourLogNotFoundException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpDelete("{tourLogGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourLogDto>> DeleteTourLog(Guid tourLogGuid)
    {
        try
        {
            return Ok(await this.tourLogService.DeleteTourLog(tourLogGuid));
        }
        catch (TourLogNotFoundException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.InnerException?.Message ?? ex.Message);
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.InnerException?.Message ?? ex.Message);
        }
    }
}