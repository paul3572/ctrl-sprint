using cts.core.svc.application.Interfaces;
using cts.core.svc.contracts;

namespace cts.core.svc.application.Services;

public class TransportService : ITransportService
{
    private readonly ITransportRepository transportRepository;

    public TransportService(ITransportRepository transportRepository)
    {
        this.transportRepository = transportRepository;
    }
    
    public async Task<List<Transport>> GetAllTransportTypes()
    {
        return await transportRepository.GetAllTransportTypes();
    }

    public async Task<Transport?> GetTransportTypeById(int id)
    {
        return await transportRepository.GetTransportTypeById(id);
    }
}