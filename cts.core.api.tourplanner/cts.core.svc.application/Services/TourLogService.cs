using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.contracts.TourLogs;
using Microsoft.AspNetCore.Mvc;

namespace cts.core.svc.application.Services;

public class TourLogService : ITourLogService
{
    private readonly ITourLogRepository tourLogRepository;

    public TourLogService(ITourLogRepository tourLogRepository)
    {
        this.tourLogRepository = tourLogRepository;
    }

    public async Task<ActionResult<List<TourLogDto>>> GetTourLogsOfTour(Guid tourGuid)
    {
        return await this.tourLogRepository.GetToursLogsOfTour(tourGuid);
    }

    public async Task<ActionResult<TourLogDto>> GetTourLog(Guid tourLogGuid)
    {
        var tourLog = await this.tourLogRepository.GetTourLog(tourLogGuid);

        return tourLog.Value ?? throw new KeyNotFoundException($"TourLog with Guid {tourLogGuid} not found.");
    }

    public async Task<ActionResult<GuidDto>> CreateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        return await this.tourLogRepository.CreateTourLog(tourGuid, tourLog);
    }

    public async Task<ActionResult<GuidDto>> UpdateTourLog(Guid tourGuid, TourLogCmd tourLog)
    {
        return await this.tourLogRepository.UpdateTourLog(tourGuid, tourLog);
    }

    public Task<ActionResult<GuidDto>> DeleteTourLog(Guid tourGuid)
    {
        return this.tourLogRepository.DeleteTourLog(tourGuid);
    }
}