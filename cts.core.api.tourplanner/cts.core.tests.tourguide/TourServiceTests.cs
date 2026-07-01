using cts.core.svc.application.Interfaces;
using cts.core.svc.application.Services;
using cts.core.svc.application.Services.OpenRoute;
using cts.core.svc.contracts.Tours;
using cts.core.svc.domain;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace cts.core.tests.tourguide;

[TestFixture]
public class TourServiceTests
{
    private Mock<ITourRepository> tourRepository = null!;
    private Mock<IRouteService> routeService = null!;
    private Mock<ITransportRepository> transportRepository = null!;

    private TourService _service = null!;

    [SetUp]
    public void Setup()
    {
        tourRepository = new Mock<ITourRepository>();
        routeService = new Mock<IRouteService>();
        transportRepository = new Mock<ITransportRepository>();

        _service = new TourService(
            tourRepository.Object,
            routeService.Object,
            transportRepository.Object);
    }

    [Test]
    public async Task CreateTour_ShouldReturnTourDto_WhenEverythingSucceeds()
    {
        // Arrange
        var userGuid = Guid.NewGuid();

        var command = new TourCmd(
            Guid.NewGuid(),
            "Nice Tour",
            "Very nice Tour",
            "Vienna",
            "Linz",
            "Car",
            5);

        var transport = new Transport("Car", "driving-car");

        var route = new RouteResult(
            100.0,
            12500,
            []
            );

        var user = User.Create(
            "test@test.at",
            "Password123",
            DateTime.UtcNow);

        var createdTour = new Tour(
            user,
            command.Name,
            command.Description,
            command.From,
            command.To,
            transport,
            route.DistanceInMeters,
            route.EstimatedTimeMin,
            command.Rating);

        transportRepository
            .Setup(x => x.GetTransportTypeByName("Car"))
            .ReturnsAsync(transport);

        routeService
            .Setup(x => x.GetRouteAsync(
                command.From,
                command.To,
                transport.OpenRouteProfile))
            .ReturnsAsync(route);

        tourRepository
            .Setup(x => x.CreateTour(
                userGuid,
                command,
                route.DistanceInMeters,
                route.EstimatedTimeMin))
            .ReturnsAsync(new ActionResult<Tour>(createdTour));

        // Act
        var result = await _service.CreateTour(userGuid, command);

        // Assert
        Assert.That(result.Name, Is.EqualTo(command.Name));
        Assert.That(result.From, Is.EqualTo(command.From));
        Assert.That(result.To, Is.EqualTo(command.To));
        Assert.That(result.TransportName, Is.EqualTo("Car"));
        Assert.That(result.TourDistanceInMeters, Is.EqualTo(route.DistanceInMeters));
        Assert.That(result.EstimatedTimeMinutes, Is.EqualTo(route.EstimatedTimeMin));

        transportRepository.Verify(x => x.GetTransportTypeByName("Car"), Times.Once);

        routeService.Verify(x => x.GetRouteAsync(
            command.From,
            command.To,
            transport.OpenRouteProfile), Times.Once);

        tourRepository.Verify(x => x.CreateTour(
            userGuid,
            command,
            route.DistanceInMeters,
            route.EstimatedTimeMin), Times.Once);
    }

    [Test]
    public void CreateTour_ShouldThrow_WhenTransportDoesNotExist()
    {
        // Arrange
        var command = new TourCmd(
            Guid.NewGuid(),
            "Tour",
            "",
            "A",
            "B",
            "Car",
            3);

        transportRepository
            .Setup(x => x.GetTransportTypeByName("Plane"))
            .ReturnsAsync((Transport?)null);

        // Act + Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.CreateTour(Guid.NewGuid(), command));

        routeService.VerifyNoOtherCalls();
        tourRepository.VerifyNoOtherCalls();
    }

    [Test]
    public void CreateTour_ShouldThrow_WhenRepositoryReturnsNull()
    {
        // Arrange
        var userGuid = Guid.NewGuid();

        var command = new TourCmd(
            Guid.NewGuid(),
            "Tour",
            "",
            "A",
            "B",
            "Car",
            2);

        var transport = new Transport("Car", "driving-car");

        var route = new RouteResult(
            1000.0,
            5,
            []);

        transportRepository
            .Setup(x => x.GetTransportTypeByName("Car"))
            .ReturnsAsync(transport);

        routeService
            .Setup(x => x.GetRouteAsync(
                command.From,
                command.To,
                transport.OpenRouteProfile))
            .ReturnsAsync(route);

        tourRepository
            .Setup(x => x.CreateTour(
                userGuid,
                command,
                route.DistanceInMeters,
                route.EstimatedTimeMin))
            .ReturnsAsync(new ActionResult<Tour>((Tour?)null));

        // Act + Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.CreateTour(userGuid, command));
    }
}