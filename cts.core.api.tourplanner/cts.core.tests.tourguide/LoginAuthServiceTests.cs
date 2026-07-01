using cts.core.svc.application.Abstractions.Authentication;
using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.application.Auth;
using cts.core.svc.application.Auth.Exceptions;
using cts.core.svc.application.Auth.Login;
using cts.core.svc.application.Services;
using cts.core.svc.domain;
using Moq;

namespace cts.core.tests.tourguide;

[TestFixture]
public class LoginAuthServiceTests
{
    private Mock<IUserRepository> userRepository = null!;
    private Mock<IAccessTokenGenerator> tokenGenerator = null!;

    private LoginAuthService _service = null!;

    [SetUp]
    public void Setup()
    {
        userRepository = new Mock<IUserRepository>();
        tokenGenerator = new Mock<IAccessTokenGenerator>();

        _service = new LoginAuthService(
            userRepository.Object,
            tokenGenerator.Object);
    }

    [Test]
    public async Task HandleAsync_Should_ReturnAuthenticationResult_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123";

        var user = User.Create(
            "test@test.com",
            password,
            DateTime.UtcNow);

        var token = new AccessToken(
            "jwt-token",
            DateTimeOffset.UtcNow.AddHours(1));

        userRepository
            .Setup(r => r.GetByNormalizedEmailAsync(
                User.NormalizeEmail("test@test.com"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        tokenGenerator
            .Setup(g => g.Generate(user))
            .Returns(token);

        var command = new LoginUserCommand(
            "test@test.com",
            password);

        // Act
        AuthenticationResult result =
            await _service.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.UserGuid, Is.EqualTo(user.UserGuid));
        Assert.That(result.Email, Is.EqualTo(user.Email));
        Assert.That(result.AccessToken, Is.EqualTo(token.Value));
        Assert.That(result.AccessTokenExpiresAtUtc, Is.EqualTo(token.ExpiresAtUtc));
    }

    [Test]
    public void HandleAsync_Should_ThrowInvalidCredentialsException_WhenUserDoesNotExist()
    {
        // Arrange
        userRepository
            .Setup(r => r.GetByNormalizedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginUserCommand(
            "test@test.com",
            "Password123");

        // Act + Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () =>
            await _service.HandleAsync(command, CancellationToken.None));
    }

    [Test]
    public void HandleAsync_Should_ThrowInvalidCredentialsException_WhenPasswordIsWrong()
    {
        // Arrange
        var user = User.Create(
            "test@test.com",
            "CorrectPassword",
            DateTime.UtcNow);

        userRepository
            .Setup(r => r.GetByNormalizedEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginUserCommand(
            "test@test.com",
            "WrongPassword");

        // Act + Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () =>
            await _service.HandleAsync(command, CancellationToken.None));
    }
}