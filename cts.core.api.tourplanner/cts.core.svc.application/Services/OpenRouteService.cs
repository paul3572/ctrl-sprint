using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using cts.core.svc.application.Interfaces;
using cts.core.svc.application.Services.OpenRoute;

namespace cts.core.svc.application.Services;

public class OpenRouteService : IRouteService
{
    private readonly HttpClient httpClient;

    public OpenRouteService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    
    public async Task<RouteResult> GetRouteAsync(string from, string to, string transportOpenRouteProfile,
        CancellationToken cancellationToken = default)
    {
        GeocodeResult start =
            await GeocodeAsync(from, cancellationToken);

        GeocodeResult end =
            await GeocodeAsync(to, cancellationToken);

        RouteRequest request = new()
        {
            Coordinates =
            [
                [start.Longitude, start.Latitude],
                [end.Longitude, end.Latitude]
            ]
        };

        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync(
                $"v2/directions/{transportOpenRouteProfile}",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        RouteResponse? route =
            await response.Content.ReadFromJsonAsync<RouteResponse>(
                cancellationToken: cancellationToken);

        if (route?.Features.Count == 0)
        {
            throw new ValidationException("No route found.");
        }
        
        if (route is null)
            throw new ValidationException("No route found.");

        RouteFeature routeFeature = route.Features[0];

        return new RouteResult(
            DistanceInMeters: routeFeature.Properties.Summary.Distance,
            EstimatedTimeMin: (int)Math.Round(routeFeature.Properties.Summary.Duration / 60.0),
            Geometry: routeFeature.RouteGeometry.Coordinates);
    }

    public async Task<GeocodeResult> GeocodeAsync(string city, CancellationToken cancellationToken = default)
    {
        GeocodeResponse? response =
            await httpClient.GetFromJsonAsync<GeocodeResponse>(
                $"geocode/search?text={Uri.EscapeDataString(city)}&size=1", cancellationToken: cancellationToken);
        
        if (response?.Features == null || response.Features.Count == 0)
        {
            throw new ValidationException($"Location '{city}' was not found.");
        }

        var feature = response.Features[0];

        return new GeocodeResult(
            Latitude: feature.Geometry.Coordinates[1],
            Longitude: feature.Geometry.Coordinates[0],
            Label: feature.GeocodeProperties.Label
        );
    }
}