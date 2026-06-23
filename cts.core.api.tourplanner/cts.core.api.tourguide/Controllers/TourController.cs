using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourController : ControllerBase, ITourController
{
    private readonly ITourService tourService;

    public TourController(ITourService tourService)
    {
        this.tourService = tourService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TourDto>>> GetToursOfUser(Guid userGuid)
    {
        try
        {
            return Ok(await this.tourService.GetToursOfUser(userGuid));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    public async Task<ActionResult<TourDto>> GetTour(Guid tourGuid)
    {
        try
        {
            return Ok(await this.tourService.GetTour(tourGuid));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    public async Task<ActionResult<Guid>> CreateTour(Guid userGuid, TourCmd tour)
    {
        try
        {
            return Ok(await this.tourService.CreateTour(userGuid, tour));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    public async Task<ActionResult<Guid>> UpdateTour(Guid tourGuid, TourCmd tour)
    {
        try
        {
            return Ok(await this.tourService.UpdateTour(tourGuid, tour));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    public async Task<ActionResult<string>> DeleteTour(Guid tourGuid)
    {
        try
        {
            return Ok(await this.tourService.DeleteTour(tourGuid));
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }
}