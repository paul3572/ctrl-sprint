using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

public class TourLogRepository(TourPlannerDbContext db) : ITourLogRepository
{
    public async Task<ActionResult<List<TourLogDto>>> GetToursLogsOfTour(Guid tourGuid)
    {
        return await db.TourLogs
            .Where(tl => tl.Tour.TourGuid == tourGuid)
            .Select(tl => new TourLogDto(
                tl.TourLogGuid,
                tl.Tour.TourGuid,
                tl.Timestamp,
                tl.Comment,
                tl.Difficulty,
                tl.TotalDistanceInMeters,
                tl.TotalTimeMin,
                tl.Rating
            ))
            .ToListAsync();
    }

    public async Task<ActionResult<TourLogDto?>> GetTourLog(Guid tourLogGuid)
    {
        return await db.TourLogs
            .Where(tl => tl.TourLogGuid == tourLogGuid)
            .Select(tl => new TourLogDto(
                tl.TourLogGuid,
                tl.Tour.TourGuid,
                tl.Timestamp,
                tl.Comment,
                tl.Difficulty,
                tl.TotalDistanceInMeters,
                tl.TotalTimeMin,
                tl.Rating
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        var tour = await db.Tours.FirstOrDefaultAsync(t => t.TourGuid == tourGuid);
        
        if (tour is null)
            throw new KeyNotFoundException($"Tour with Guid {tourGuid} not found.");

        var creatingTourLog = new TourLog(
            tour,
            tourLog.Timestamp,
            tourLog.Comment,
            tourLog.Difficulty,
            tourLog.TotalDistanceInMeters,
            tourLog.TotalTimeMin,
            tourLog.Rating
        );

        var createdTourLog = db.TourLogs.Add(creatingTourLog);
        await db.SaveChangesAsync();

        return new GuidDto(createdTourLog.Entity.TourLogGuid);
    }

    public async Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog)
    {
        var updatingTourLog = await db.TourLogs.FirstOrDefaultAsync(tl => tl.TourLogGuid == tourLogGuid);
        
        if (updatingTourLog is null)
            throw new KeyNotFoundException($"TourLog with Guid {tourLogGuid} not found.");

        updatingTourLog.Timestamp = tourLog.Timestamp;
        updatingTourLog.Comment = tourLog.Comment;
        updatingTourLog.Difficulty = tourLog.Difficulty;
        updatingTourLog.TotalDistanceInMeters = tourLog.TotalDistanceInMeters;
        updatingTourLog.TotalTimeMin = tourLog.TotalTimeMin;
        updatingTourLog.Rating = tourLog.Rating;
        
        await db.SaveChangesAsync();

        return new GuidDto(updatingTourLog.TourLogGuid);
    }

    public async Task<ActionResult<GuidDto>> DeleteTourLog(Guid tourLogGuid)
    {
        var tourLog = await db.TourLogs.FirstOrDefaultAsync(tl => tl.TourLogGuid == tourLogGuid);
        
        if (tourLog is null)
            throw new KeyNotFoundException($"TourLog with Guid {tourLogGuid} not found.");

        var deletedTourLog = db.TourLogs.Remove(tourLog);
        await db.SaveChangesAsync();

        return new GuidDto(deletedTourLog.Entity.TourLogGuid);
    }
}