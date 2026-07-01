using cts.core.svc.application.Interfaces;
using cts.core.svc.domain.OpenRoute;

namespace cts.core.svc.application.Services;

public class MockRouteService : IRouteService
{
    public Task<RouteResult> GetRouteAsync(string from, string to, string transportOpenRouteProfile, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new RouteResult(
                17.5,
                28,
                new RouteGeometry()
            ));
    }

    public Task<GeocodeResult> GeocodeAsync(string city, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}