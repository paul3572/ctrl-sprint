using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.contracts;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence;

public class TourPlannerDbContext : DbContext, IUnitOfWork
{
    public TourPlannerDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourLog> TourLogs { get; set; }
    public DbSet<Transport> Transports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Tour>().ToTable("Tour");
        modelBuilder.Entity<TourLog>().ToTable("TourLog");
        modelBuilder.Entity<Transport>().ToTable("Transport");

        modelBuilder.Entity<User>().HasAlternateKey(u => u.UserGuid);
        modelBuilder.Entity<Tour>().HasAlternateKey(t => t.TourGuid);
        modelBuilder.Entity<TourLog>().HasAlternateKey(tl => tl.TourLogGuid);
        
        modelBuilder.Entity<User>().Property(u => u.UserGuid).ValueGeneratedOnAdd();
        modelBuilder.Entity<Tour>().Property(t => t.TourGuid).ValueGeneratedOnAdd();
        modelBuilder.Entity<TourLog>().Property(tl => tl.TourLogGuid).ValueGeneratedOnAdd();
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tours)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Tour>()
            .HasMany(t => t.TourLogs)
            .WithOne(t => t.Tour)
            .HasForeignKey(t => t.TourId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Transport>()
            .HasMany(t => t.Tours)
            .WithOne(t => t.Transport)
            .HasForeignKey(t => t.TransportId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public void Initialize(bool deleteDatabase = false)
    {
        if (deleteDatabase)
            this.Database.EnsureDeleted();
        
        this.Database.EnsureCreated();
    }
}