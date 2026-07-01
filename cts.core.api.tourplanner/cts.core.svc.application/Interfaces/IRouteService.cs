using cts.core.svc.domain.OpenRoute;

namespace cts.core.svc.application.Interfaces;

public interface IRouteService
{
    Task<RouteResult> GetRouteAsync(string from, string to, string transportOpenRouteProfile, CancellationToken cancellationToken = default);

    Task<GeocodeResult> GeocodeAsync(string city, CancellationToken cancellationToken = default);
}