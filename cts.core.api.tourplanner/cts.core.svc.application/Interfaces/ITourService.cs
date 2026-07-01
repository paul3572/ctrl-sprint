using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Interfaces;

public interface ITourService
{
    Task<List<TourDto>> GetToursOfUser(Guid userGuid);
    Task<TourDto> GetTour(Guid tourGuid);
    Task<TourDto> CreateTour(Guid userGuid, TourCmd tour);
    Task<TourDto> UpdateTour(Guid tourGuid, TourCmd tour);
    Task<TourDto> DeleteTour(Guid tourGuid);
    Task<List<TourDto>> BuyData(Guid userGuid, List<TourDto> tours);
}