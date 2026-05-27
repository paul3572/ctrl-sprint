using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner.Application.Abstractions.Authentication;
using TourPlanner.Application.Abstractions.Persistence;
using TourPlanner.Application.Abstractions.Time;
using TourPlanner.Infrastructure.Authentication;
using TourPlanner.Infrastructure.Persistence;
using TourPlanner.Infrastructure.Persistence.Repositories;
using TourPlanner.Infrastructure.Time;

namespace TourPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
                                  ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        services.AddDbContext<TourPlannerDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "app");
                });
        });

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<TourPlannerDbContext>());

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHashingService, AspNetPasswordHashingService>();
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

        return services;
    }
}