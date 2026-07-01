using cts.core.svc.application.Interfaces;
using cts.core.svc.domain;
using cts.core.svc.infrastructure.Persistence;
using cts.core.svc.infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace cts.core.tests.tourguide;

public class TourRepositoryTests
{
    private TourPlannerDbContext db;
    private TourRepository repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        db = new TourPlannerDbContext(options);

        var transportRepoMock = new Mock<ITransportRepository>();

        repository = new TourRepository(transportRepoMock.Object, db);
    }

    [TearDown]
    public void TearDown()
    {
        db.Dispose();
    }

    [Test]
    public async Task GetTour_ShouldReturnTour_WithIncludes()
    {
        var user = new User("test@test.com", "password", DateTime.UtcNow);

        var transport = new Transport("Car", "driving-car");

        var tour = new Tour(
            user,
            "Tour 1",
            "Desc",
            "Vienna",
            "Linz",
            transport,
            10000,
            120,
            4
        );

        db.Users.Add(user);
        db.Tours.Add(tour);
        await db.SaveChangesAsync();

        var result = await repository.GetTour(tour.TourGuid);

        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.User.Email, Is.EqualTo("TEST@TEST.COM"));
        Assert.That(result.Value.Transport.Name, Is.EqualTo("Car"));
    }

    [Test]
    public async Task GetTour_ShouldReturnNull_WhenTourDoesNotExist()
    {
        var result = await repository.GetTour(Guid.NewGuid());

        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public async Task GetTour_ShouldIncludeTourLogs()
    {
        var user = new User("test@test.com", "password", DateTime.UtcNow);
        var transport = new Transport("Car", "driving-car");

        var tour = new Tour(
            user,
            "Tour 1",
            "Desc",
            "Vienna",
            "Linz",
            transport,
            10000,
            120,
            4
        );

        var tourLog = new TourLog(
            tour,
            DateTime.UtcNow,
            "Nice tour",
            3,
            10000,
            120,
            4
        );

        db.TourLogs.Add(tourLog);
        db.Users.Add(user);
        db.Tours.Add(tour);
        await db.SaveChangesAsync();

        var result = await repository.GetTour(tour.TourGuid);

        Assert.That(result.Value?.TourLogs, Has.Count.EqualTo(1));
    }
}