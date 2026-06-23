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
        modelBuilder.Entity<User>().ToTable("app_user");
        modelBuilder.Entity<Tour>().ToTable("tour");
        modelBuilder.Entity<TourLog>().ToTable("tourLog");
        modelBuilder.Entity<Transport>().ToTable("transport");

        modelBuilder.Entity<User>().HasAlternateKey(u => u.UserGuid);
        modelBuilder.Entity<Tour>().HasAlternateKey(t => t.TourGuid);
        modelBuilder.Entity<TourLog>().HasAlternateKey(tl => tl.TourLogGuid);
        
        modelBuilder.Entity<User>().Property(u => u.UserGuid).ValueGeneratedOnAdd();
        modelBuilder.Entity<Tour>().Property(t => t.TourGuid).ValueGeneratedOnAdd();
        modelBuilder.Entity<TourLog>().Property(tl => tl.TourLogGuid).ValueGeneratedOnAdd();
        
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        
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

    public void Seed()
    {
        var user1 = new User("linus@test.at", "linus", DateTime.UtcNow);
        var user2 = new User("paul@test.at", "paul", DateTime.UtcNow);

        this.Users.AddRange(user1, user2);
        this.SaveChanges();
        
        var transport1 = new Transport("Car");
        var transport2 = new Transport("Bike");
        var transport3 = new Transport("By Foot");
        var transport4 = new Transport("Public Transport");
        
        this.Transports.AddRange(transport1, transport2, transport3, transport4);
        this.SaveChanges();

        var tour1 = new Tour(user1, transport1, 5, 5, 5);
        var tour2 = new Tour(user1, transport3, 5, 5, 5);
        var tour3 = new Tour(user2, transport2, 5, 5, 5);
        var tour4 = new Tour(user2, transport3, 5, 5, 5);
        
        this.Tours.AddRange(tour1, tour2, tour3, tour4);
        this.SaveChanges();
        
        var tourLog1 = new TourLog(tour1, DateTime.UtcNow, "Great tour!", 5, 5, 5, 5);
        var tourLog2 = new TourLog(tour2,  DateTime.UtcNow, "Great tour!", 5, 5, 5, 5);
        var tourLog3 = new TourLog(tour3, DateTime.UtcNow, "Great tour!", 5, 5, 5, 5);
        var tourLog4 = new  TourLog(tour4, DateTime.UtcNow, "Great tour!", 5, 5, 5, 5);
        
        this.TourLogs.AddRange(tourLog1, tourLog2, tourLog3, tourLog4);
        this.SaveChanges();
    }
}