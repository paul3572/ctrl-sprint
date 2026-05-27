using Microsoft.EntityFrameworkCore;
using TourPlanner.Application.Abstractions.Persistence;
using TourPlanner.Domain.Users;

namespace TourPlanner.Infrastructure.Persistence;

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