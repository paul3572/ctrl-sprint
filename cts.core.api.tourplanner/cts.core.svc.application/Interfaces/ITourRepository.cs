using cts.core.svc.contracts.Tours;
using cts.core.svc.domain;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourRepository
{
    Task<List<TourDto>> GetToursOfUser(Guid userGuid);
    Task<ActionResult<Tour?>> GetTour(Guid tourGuid);
    Task<ActionResult<Tour>> CreateTour(Guid userGuid, TourCmd tour, double distanceInMeters, int estimatedTimeMin);
    Task<ActionResult<Tour>> UpdateTour(Guid tourGuid, TourCmd tour);
    Task<ActionResult<Tour>> DeleteTour(Guid tourGuid);
}