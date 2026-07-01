using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Auth;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.application.Auth.Register;
using cts.core.svc.application.Services;
using cts.core.svc.domain;
using Moq;

namespace cts.core.tests.tourguide;

[TestFixture]
public class RegisterAuthServiceTests
{
    private Mock<IUserRepository> userRepository = null!;
    private Mock<IUnitOfWork> unitOfWork = null!;
    private Mock<IAccessTokenGenerator> tokenGenerator = null!;

    private RegisterAuthService service = null!;

    [SetUp]
    public void Setup()
    {
        userRepository = new Mock<IUserRepository>();
        unitOfWork = new Mock<IUnitOfWork>();
        tokenGenerator = new Mock<IAccessTokenGenerator>();

        service = new RegisterAuthService(
            userRepository.Object,
            unitOfWork.Object,
            tokenGenerator.Object);
    }

    [Test]
    public async Task HandleAsync_Should_RegisterUser_WhenInputIsValid()
    {
        // Arrange
        userRepository
            .Setup(r => r.ExistsByNormalizedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var token = new AccessToken(
            "jwt-token",
            DateTimeOffset.UtcNow.AddHours(1));

        tokenGenerator
            .Setup(t => t.Generate(It.IsAny<User>()))
            .Returns(token);

        var command = new RegisterUserCommand(
            "test@test.com",
            "Password123");

        // Act
        AuthenticationResult result =
            await service.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Email, Is.EqualTo("TEST@TEST.COM"));
        Assert.That(result.AccessToken, Is.EqualTo(token.Value));

        userRepository.Verify(r =>
            r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        tokenGenerator.Verify(t =>
            t.Generate(It.IsAny<User>()),
            Times.Once);
    }

    [Test]
    public void HandleAsync_Should_ThrowPasswordPolicyViolationException_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@test.com",
            "123");

        // Act + Assert
        Assert.ThrowsAsync<PasswordPolicyViolationException>(async () =>
            await service.HandleAsync(command, CancellationToken.None));
    }

    [Test]
    public void HandleAsync_Should_ThrowEmailAlreadyRegisteredException_WhenUserAlreadyExists()
    {
        // Arrange
        userRepository
            .Setup(r => r.ExistsByNormalizedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand(
            "test@test.com",
            "Password123");

        // Act + Assert
        Assert.ThrowsAsync<EmailAlreadyRegisteredException>(async () =>
            await service.HandleAsync(command, CancellationToken.None));

        userRepository.Verify(r =>
            r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task HandleAsync_Should_CallRepositoryAndSaveChangesOnce()
    {
        // Arrange
        userRepository
            .Setup(r => r.ExistsByNormalizedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        tokenGenerator
            .Setup(t => t.Generate(It.IsAny<User>()))
            .Returns(new AccessToken(
                "jwt-token",
                DateTimeOffset.UtcNow.AddHours(1)));

        var command = new RegisterUserCommand(
            "test@test.com",
            "Password123");

        // Act
        await service.HandleAsync(command, CancellationToken.None);

        // Assert
        userRepository.Verify(r =>
            r.ExistsByNormalizedEmailAsync(
                User.NormalizeEmail("test@test.com"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        userRepository.Verify(r =>
            r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(u =>
            u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        tokenGenerator.Verify(t =>
            t.Generate(It.IsAny<User>()),
            Times.Once);
    }
}