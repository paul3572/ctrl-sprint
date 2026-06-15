using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence;

public sealed class TourPlannerDbContext : DbContext, IUnitOfWork
{
    public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourPlannerDbContext).Assembly);
    }
}