using System.ComponentModel.DataAnnotations;
using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.Tours;
using cts.core.svc.domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourController : ControllerBase, ITourController
{
    private readonly ITourService tourService;
    private readonly ILogger<TourController> logger;

    public TourController(ITourService tourService, ILogger<TourController> logger)
    {
        this.tourService = tourService;
        this.logger = logger;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TourDto>>> GetToursOfUser(Guid userGuid)
    {
        try
        {
            return Ok(await this.tourService.GetToursOfUser(userGuid));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error while creating a tour.");

            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
    }

    [HttpGet("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TourDto>> GetTour(Guid tourGuid)
    {
        try
        {
            return Ok(await this.tourService.GetTour(tourGuid));
        }
        catch (TourException ex)
        {
            this.logger.LogWarning(ex, "Something went wrong while retrieving Tour {TourGuid}.", tourGuid);
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "Validation failed while creating a tour.");
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    [HttpPost("{userGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TourDto>> CreateTour(Guid userGuid, TourCmd tour)
    {
        try
        {
            return Ok(await this.tourService.CreateTour(userGuid, tour));
        }
        catch (UserNotFoundException ex)
        {
            this.logger.LogWarning(ex, "User {UserGuid} not found while creating a tour.", userGuid);
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (TransportException ex)
        {
            this.logger.LogWarning(ex, "Transport type could not be resolved.");
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (TourException ex)
        {
            this.logger.LogWarning(ex, "Validation failed while creating a tour.");
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (ValidationException ex)
        {
            this.logger.LogWarning(ex, "Validation failed while creating a tour.");
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.InnerException?.Message ?? ex.Message);
        }
        catch  (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error while creating a tour.");
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    [HttpPatch("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TourDto>> UpdateTour(Guid tourGuid, TourCmd tour)
    {
        try
        {
            return Ok(await this.tourService.UpdateTour(tourGuid, tour));
        }
        catch (TourNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Tour {tourGuid} not found.", tourGuid);
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (TourException ex)
        {
            this.logger.LogWarning(ex, "Validation failed while updating a tour.");
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "Validation failed while updating a tour.");
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    [HttpDelete("{tourGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TourDto>> DeleteTour(Guid tourGuid)
    {
        try
        {
            return Ok(await this.tourService.DeleteTour(tourGuid));
        }
        catch (TourNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Tour {UserGuid} not found while deleting a tour.", tourGuid);

            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (TourException ex)
        {
            this.logger.LogWarning(ex, "Validation failed while deleting a tour.");
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch  (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
    }

    [HttpPost("buyData/{userGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TourDto>>> BuyData(Guid userGuid, [FromBody] List<TourDto> tours)
    {
        try
        {
            return Ok(await this.tourService.BuyData(userGuid, tours));
        }
        catch (UserNotFoundException ex)
        {
            this.logger.LogWarning(ex, "User {UserGuid} not found while importing.", userGuid);
            
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (TourException ex)
        {
            this.logger.LogWarning(ex, "Validation failed while importing.");
            
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error while importing.");

            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
    }
}