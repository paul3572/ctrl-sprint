using cts.core.svc.contracts.Transports;
using Microsoft.AspNetCore.Mvc;

namespace TourGuideApplication.Interfaces;

public interface ITransportController
{
    Task<ActionResult<List<TransportDto>>> GetAllTransportTypes();
    
    Task<ActionResult<TransportDto>> GetTransportTypeById(int id);
}