using cts.core.svc.contracts;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace TourGuideApplication.Interfaces;

public interface ITourController
{
    Task<ActionResult<List<TourDto>>> GetToursOfUser(Guid userGuid);
    Task<ActionResult<TourDto>> GetTour(Guid tourGuid);
    Task<ActionResult<TourDto>> CreateTour(Guid userGuid, TourCmd tour);
    Task<ActionResult<TourDto>> UpdateTour(Guid tourGuid, TourCmd tour);
    Task<ActionResult<TourDto>> DeleteTour(Guid tourGuid);
    Task<ActionResult<List<TourDto>>> BuyData(Guid userGuid, List<TourDto> tours);
}