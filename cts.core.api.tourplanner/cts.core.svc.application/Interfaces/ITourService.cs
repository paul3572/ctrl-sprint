using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourService
{
    Task<List<TourDto>> GetToursOfUser(Guid userGuid);
    Task<ActionResult<TourDto>> GetTour(Guid tourGuid);
    Task<ActionResult<TourDto>> CreateTour(Guid userGuid, TourCmd tour);
    Task<ActionResult<TourDto>> UpdateTour(Guid tourGuid, TourCmd tour);
    Task<ActionResult<TourDto>> DeleteTour(Guid tourGuid);
}