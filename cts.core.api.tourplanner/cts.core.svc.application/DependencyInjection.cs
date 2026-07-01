using cts.core.svc.application.Auth.Login;
using cts.core.svc.application.Auth.Register;
using cts.core.svc.application.Interfaces;
using cts.core.svc.application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cts.core.svc.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterAuthService>();
        services.AddScoped<LoginAuthService>();
        services.AddScoped<ITransportService, TransportService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<ITourLogService, TourLogService>();
        services.AddScoped<IWeatherService, OpenMeteoService>();

        return services;
    }
}