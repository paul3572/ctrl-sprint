using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using cts.core.svc.domain;
using cts.core.svc.domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

public class TourLogRepository(TourPlannerDbContext db) : ITourLogRepository
{
    public async Task<List<TourLogDto>> GetToursLogsOfTour(Guid tourGuid)
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

    public async Task<TourLogDto?> GetTourLog(Guid tourLogGuid)
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

    public async Task<TourLogDto> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        var tour = await db.Tours.FirstOrDefaultAsync(t => t.TourGuid == tourGuid);

        if (tour is null)
            throw new TourNotFoundException($"Tour with Guid {tourGuid} not found.");

        var creatingTourLog = new TourLog(
            tour,
            tourLog.Timestamp,
            tourLog.Comment,
            tourLog.Difficulty,
            tourLog.TotalDistanceInMeters,
            tourLog.TotalTimeMin,
            tourLog.Rating
        );

        var createdTourLog = db.TourLogs.Add(creatingTourLog).Entity;
        await db.SaveChangesAsync();

        return new TourLogDto(
            createdTourLog.TourLogGuid,
            createdTourLog.Tour.TourGuid,
            createdTourLog.Timestamp,
            createdTourLog.Comment,
            createdTourLog.Difficulty,
            createdTourLog.TotalDistanceInMeters,
            createdTourLog.TotalTimeMin,
            createdTourLog.Rating
        );
    }

    public async Task<TourLogDto> UpdateTourLog(Guid tourLogGuid, TourLogCmd tourLog)
    {
        var updatingTourLog = await db.TourLogs
            .Include(t => t.Tour)
            .FirstOrDefaultAsync(tl => tl.TourLogGuid == tourLogGuid);

        if (updatingTourLog is null)
            throw new TourLogNotFoundException($"TourLog with Guid {tourLogGuid} not found.");

        updatingTourLog.Timestamp = tourLog.Timestamp;
        updatingTourLog.Comment = tourLog.Comment;
        updatingTourLog.Difficulty = tourLog.Difficulty;
        updatingTourLog.TotalDistanceInMeters = tourLog.TotalDistanceInMeters;
        updatingTourLog.TotalTimeMin = tourLog.TotalTimeMin;
        updatingTourLog.Rating = tourLog.Rating;

        await db.SaveChangesAsync();

        return new TourLogDto(
            updatingTourLog.TourLogGuid,
            updatingTourLog.Tour.TourGuid,
            updatingTourLog.Timestamp,
            updatingTourLog.Comment,
            updatingTourLog.Difficulty,
            updatingTourLog.TotalDistanceInMeters,
            updatingTourLog.TotalTimeMin,
            updatingTourLog.Rating
        );
    }

    public async Task<TourLogDto> DeleteTourLog(Guid tourLogGuid)
    {
        var tourLog = await db.TourLogs
            .Include(t => t.Tour)
            .FirstOrDefaultAsync(tl => tl.TourLogGuid == tourLogGuid);

        if (tourLog is null)
            throw new TourLogNotFoundException($"TourLog with Guid {tourLogGuid} not found.");

        var deletedTourLog = db.TourLogs.Remove(tourLog).Entity;
        await db.SaveChangesAsync();

        return new TourLogDto(
            deletedTourLog.TourLogGuid,
            deletedTourLog.Tour.TourGuid,
            deletedTourLog.Timestamp,
            deletedTourLog.Comment,
            deletedTourLog.Difficulty,
            deletedTourLog.TotalDistanceInMeters,
            deletedTourLog.TotalTimeMin,
            deletedTourLog.Rating
        );
    }
}