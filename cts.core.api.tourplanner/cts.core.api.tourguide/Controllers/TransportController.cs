using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.Transports;
using Microsoft.AspNetCore.Mvc;
using TourGuideApplication.Interfaces;

namespace TourGuideApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransportController : ControllerBase, ITransportController
{
    private readonly ITransportService transportService;

    public TransportController(ITransportService transportService)
    {
        this.transportService = transportService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransportDto>>> GetAllTransportTypes()
    {
        return (await this.transportService.GetAllTransportTypes())
            .Select(t => new TransportDto(
                t.TransportId,
                t.Name
            )).ToList();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransportDto>> GetTransportTypeById(int id)
    {
        var transport = await this.transportService.GetTransportTypeById(id);

        return transport is not null
            ? Ok(new TransportDto(transport.TransportId, transport.Name))
            : Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Transport with Id {id} not found.");
    }
}