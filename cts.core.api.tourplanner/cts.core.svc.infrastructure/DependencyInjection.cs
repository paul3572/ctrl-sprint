using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Abstractions.Time;
using cts.core.svc.application.Interfaces;
using cts.core.svc.application.Services;
using cts.core.svc.infrastructure.Authentication;
using cts.core.svc.infrastructure.Persistence;
using cts.core.svc.infrastructure.Persistence.Repositories;
using cts.core.svc.infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cts.core.svc.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
                                  ?? throw new InvalidOperationException(
                                      "Connection string 'Default' is missing.");

        services.AddDbContext<TourPlannerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransportRepository, TransportRepository>();

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<TourPlannerDbContext>());

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

        return services;
    }
}