using Microsoft.Extensions.DependencyInjection;
using TourPlanner.Application.Auth.Login;
using TourPlanner.Application.Auth.Register;

namespace TourPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserCommandHandler>();
        services.AddScoped<LoginUserCommandHandler>();

        return services;
    }
}