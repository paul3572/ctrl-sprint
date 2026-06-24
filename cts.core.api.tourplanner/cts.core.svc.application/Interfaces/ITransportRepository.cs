using cts.core.svc.contracts;
using cts.core.svc.domain;

namespace cts.core.svc.application.Interfaces;

public interface ITransportRepository
{
    Task<List<Transport>> GetAllTransportTypes();
    
    Task<Transport?> GetTransportTypeById(int id);
    
    Task<Transport?> GetTransportTypeByName(string name);
}