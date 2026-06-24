using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;
using cts.core.svc.domain;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

public class TransportRepository(TourPlannerDbContext dbContext) : ITransportRepository
{
    public async Task<List<Transport>> GetAllTransportTypes()
    {
        return await dbContext.Transports
            .OrderBy(t => t.TransportId)
            .ToListAsync();
    }

    public async Task<Transport?> GetTransportTypeById(int id)
    {
        return await dbContext.Transports
            .Where(t => t.TransportId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Transport?> GetTransportTypeByName(string name)
    {
        return await dbContext.Transports
            .Where(t => t.Name == name)
            .FirstOrDefaultAsync();
    }
}