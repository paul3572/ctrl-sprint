using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cts.core.svc.infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<TourPlannerDbContext>
{
    public TourPlannerDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("TOURPLANNER_MIGRATIONS_CONNECTION")
                                  ?? throw new InvalidOperationException(
                                      "Environment variable 'TOURPLANNER_MIGRATIONS_CONNECTION' is missing.");
        

        var optionsBuilder = new DbContextOptionsBuilder<TourPlannerDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "app");
            });

        return new TourPlannerDbContext(optionsBuilder.Options);
    }
}