using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts.Tours;
using cts.core.svc.domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourLogDto = cts.core.svc.contracts.TourLogs.TourLogDto;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

public class TourRepository(ITransportRepository transportRepo, TourPlannerDbContext db) : ITourRepository
{
    public async Task<ActionResult<List<TourDto>>> GetToursOfUser(Guid userGuid)
    {
        return await db.Tours
            .Include(t => t.User)
            .Include(t => t.Transport)
            .Include(t => t.TourLogs)
            .Select(t => new TourDto(
                t.TourGuid,
                t.User.UserGuid,
                t.Name,
                t.Description,
                t.From,
                t.To,
                t.Transport.Name,
                t.TourDistanceInMeters,
                t.EstimatedTimeMinutes,
                t.Rating,
                t.TourLogs.Select(tl => new TourLogDto(
                    tl.TourLogGuid,
                    tl.Tour.TourGuid,
                    tl.Timestamp,
                    tl.Comment,
                    tl.Difficulty,
                    tl.TotalDistanceInMeters,
                    tl.TotalTimeMin,
                    tl.Rating
                    )).ToList()
                )).ToListAsync();
    }

    public async Task<ActionResult<Tour?>> GetTour(Guid tourGuid)
    {
        return await db.Tours
            .Include(t => t.User)
            .Include(t => t.Transport)
            .Include(t => t.TourLogs)
            .FirstOrDefaultAsync(t => t.TourGuid == tourGuid);
    }

    public async Task<ActionResult<Guid>> CreateTour(Guid userGuid, TourCmd tour, double distanceInMeters, int estimatedTimeMin)
    {
        var user = await db.Users.FirstOrDefaultAsync(u  => u.UserGuid == userGuid);
        
        if (user is null)
            throw new KeyNotFoundException($"User with Guid {userGuid} not found.");

        var creatingTour = new Tour(
            user,
            await transportRepo.GetTransportTypeByName(tour.TransportName) ??
            await transportRepo.GetTransportTypeById(1) ?? new Transport("Car", "driving-car"),
            distanceInMeters,
            estimatedTimeMin,
            tour.Rating
        );
        
        var createdTour = db.Tours.Add(creatingTour);
        await db.SaveChangesAsync();

        return createdTour.Entity.TourGuid;
    }

    public async Task<ActionResult<Guid>> UpdateTour(Guid tourGuid, TourCmd updatingTour)
    {
        var tour = await db.Tours.FirstOrDefaultAsync(t => t.TourGuid == tourGuid);
        
        if (tour is null)
            throw new KeyNotFoundException($"Tour with Guid {tourGuid} not found.");
        
        tour.Name = updatingTour.Name;
        tour.Description = updatingTour.Description;
        tour.From = updatingTour.From;
        tour.To = updatingTour.To;
        tour.Transport = await transportRepo.GetTransportTypeByName(updatingTour.TransportName) ?? await transportRepo.GetTransportTypeById(1) ?? new Transport("Car", "driving_car");
        tour.Rating = updatingTour.Rating;
        
        await db.SaveChangesAsync();

        return tour.TourGuid;
    }

    public async Task<ActionResult<string>> DeleteTour(Guid tourGuid)
    {
        var tour = await db.Tours.FirstOrDefaultAsync(t => t.TourGuid == tourGuid);
        
        if (tour is null)
        {
            throw new KeyNotFoundException($"Tour with Guid {tourGuid} not found.");
        }

        var deletedTour = db.Tours.Remove(tour);
        await db.SaveChangesAsync();
        return deletedTour.Entity.Name;
    }
}