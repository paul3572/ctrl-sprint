using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Services;

public class TourService : ITourService
{
    private readonly ITourRepository tourRepository;
    
    public TourService(ITourRepository tourRepository)
    {
        this.tourRepository = tourRepository;
    }

    public async Task<ActionResult<List<TourDto>>> GetToursOfUser(Guid userGuid)
    {
        return await this.tourRepository.GetToursOfUser(userGuid);
    }

    public async Task<ActionResult<TourDto>> GetTour(Guid tourGuid)
    {
        var tour = await this.tourRepository.GetTour(tourGuid);
        
        if (tour?.Value is null)
            throw new KeyNotFoundException($"Tour with Guid {tourGuid} not found.");

        var tourDto = tour.Value;
        
        return new TourDto(
            tourDto.TourGuid,
            tourDto.User.UserGuid,
            tourDto.Name,
            tourDto.Description,
            tourDto.From,
            tourDto.To,
            tourDto.Transport.Name,
            tourDto.TourDistanceKm,
            tourDto.EstimatedTimeMinutes,
            tourDto.Rating,
            tourDto.TourLogs.Select(log => new TourLogDto(
                log.TourLogGuid,
                log.Tour.TourGuid,
                log.Timestamp,
                log.Comment,
                log.Difficulty,
                log.TotalDistanceKm,
                log.TotalTimeMin,
                log.Rating
            )).ToList()
        );
    }

    public async Task<ActionResult<Guid>> CreateTour(Guid userGuid, TourCmd tour)
    {
        return await this.tourRepository.CreateTour(userGuid, tour);
    }

    public async Task<ActionResult<Guid>> UpdateTour(Guid tourGuid, TourCmd tour)
    {
        return await this.tourRepository.UpdateTour(tourGuid, tour);
    }

    public async Task<ActionResult<string>> DeleteTour(Guid tourGuid)
    {
        return await this.tourRepository.DeleteTour(tourGuid);
    }
}