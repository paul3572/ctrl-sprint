using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.contracts.Tours;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Services;

public class TourService : ITourService
{
    private readonly ITourRepository tourRepository;
    private readonly IRouteService routeService;
    private readonly ITransportRepository transportRepository;
    
    public TourService(ITourRepository tourRepository, IRouteService routeService, ITransportRepository transportRepository)
    {
        this.tourRepository = tourRepository;
        this.routeService = routeService;
        this.transportRepository = transportRepository;
    }

    public async Task<List<TourDto>> GetToursOfUser(Guid userGuid)
    {
        return await this.tourRepository.GetToursOfUser(userGuid);
    }

    public async Task<ActionResult<TourDto>> GetTour(Guid tourGuid)
    {
        var tour = await this.tourRepository.GetTour(tourGuid);
        
        if (tour.Value is null)
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
            tourDto.TourDistanceInMeters,
            tourDto.EstimatedTimeMinutes,
            tourDto.Rating,
            tourDto.TourLogs.Select(log => new TourLogDto(
                log.TourLogGuid,
                log.Tour.TourGuid,
                log.Timestamp,
                log.Comment,
                log.Difficulty,
                log.TotalDistanceInMeters,
                log.TotalTimeMin,
                log.Rating
            )).ToList()
        );
    }

    public async Task<ActionResult<TourDto>> CreateTour(Guid userGuid, TourCmd tour)
    {
        var transport = await this.transportRepository.GetTransportTypeByName(tour.TransportName);
        
        if (transport is null)
            throw new KeyNotFoundException($"Transport type {tour.TransportName} not found.");
        
        var routeResult = await this.routeService.GetRouteAsync(tour.From, tour.To, transport.OpenRouteProfile);
        
        var createdTour = await this.tourRepository.CreateTour(userGuid, tour, routeResult.DistanceInMeters, routeResult.EstimatedTimeMin);

        if (createdTour.Value is null)
            throw new KeyNotFoundException($"Tour could not be created for user with Guid {userGuid}.");
        
        return new TourDto(
            createdTour.Value.TourGuid,
            createdTour.Value.User.UserGuid,
            createdTour.Value.Name,
            createdTour.Value.Description,
            createdTour.Value.From,
            createdTour.Value.To,
            createdTour.Value.Transport.Name,
            createdTour.Value.TourDistanceInMeters,
            createdTour.Value.EstimatedTimeMinutes,
            createdTour.Value.Rating,
            []
        );
    }

    public async Task<ActionResult<TourDto>> UpdateTour(Guid tourGuid, TourCmd tour)
    {
        var updatedTour = await this.tourRepository.UpdateTour(tourGuid, tour);

        if (updatedTour.Value is null)
            throw new KeyNotFoundException($"Tour could not be updated for tour with Guid {tourGuid}.");
        
        return new TourDto(
            updatedTour.Value.TourGuid,
            updatedTour.Value.User.UserGuid,
            updatedTour.Value.Name,
            updatedTour.Value.Description,
            updatedTour.Value.From,
            updatedTour.Value.To,
            updatedTour.Value.Transport.Name,
            updatedTour.Value.TourDistanceInMeters,
            updatedTour.Value.EstimatedTimeMinutes,
            updatedTour.Value.Rating,
            updatedTour.Value.TourLogs.Select(log => new TourLogDto(
                log.TourLogGuid,
                log.Tour.TourGuid,
                log.Timestamp,
                log.Comment,
                log.Difficulty,
                log.TotalDistanceInMeters,
                log.TotalTimeMin,
                log.Rating
            )).ToList()
        );
    }

    public async Task<ActionResult<TourDto>> DeleteTour(Guid tourGuid)
    {
        var deletedTour = await this.tourRepository.DeleteTour(tourGuid);

        if (deletedTour.Value is null)
            throw new KeyNotFoundException($"Tour could not be deleted for tour with Guid {tourGuid}.");
        
        return new TourDto(
            deletedTour.Value.TourGuid,
            deletedTour.Value.User.UserGuid,
            deletedTour.Value.Name,
            deletedTour.Value.Description,
            deletedTour.Value.From,
            deletedTour.Value.To,
            deletedTour.Value.Transport.Name,
            deletedTour.Value.TourDistanceInMeters,
            deletedTour.Value.EstimatedTimeMinutes,
            deletedTour.Value.Rating,
            deletedTour.Value.TourLogs.Select(log => new TourLogDto(
                log.TourLogGuid,
                log.Tour.TourGuid,
                log.Timestamp,
                log.Comment,
                log.Difficulty,
                log.TotalDistanceInMeters,
                log.TotalTimeMin,
                log.Rating
            )).ToList()
        );
    }
}