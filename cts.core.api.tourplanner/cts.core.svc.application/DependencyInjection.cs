using cts.core.svc.application.Auth.Login;
using cts.core.svc.application.Auth.Register;
using Microsoft.Extensions.DependencyInjection;

namespace cts.core.svc.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserCommandHandler>();
        services.AddScoped<LoginUserCommandHandler>();

        return services;
    }
}