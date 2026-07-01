using cts.core.svc.domain;

namespace cts.core.tests.tourguide;

[TestFixture]
public class UserTests
{
    [Test]
    public void NormalizeEmail_ShouldTrimWhitespaceAndConvertToUppercase()
    {
        // Arrange
        const string email = "  test@example.com ";

        // Act
        string normalized = User.NormalizeEmail(email);

        // Assert
        Assert.That(normalized, Is.EqualTo("TEST@EXAMPLE.COM"));
    }

    [Test]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        DateTime createdAt = DateTime.UtcNow;

        // Act
        User user = User.Create(
            "test@example.com",
            "Password123",
            createdAt);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(user.Email, Is.EqualTo("TEST@EXAMPLE.COM"));
            Assert.That(user.CreatedAtUtc, Is.EqualTo(createdAt));
            Assert.That(user.UserGuid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(user.PasswordHash, Is.Not.Empty);
            Assert.That(user.Salt, Is.Not.Empty);
        });
    }

    [Test]
    public void Create_WithEmptyEmail_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                "",
                "Password123",
                DateTime.UtcNow));
    }

    [Test]
    public void Create_WithInvalidEmail_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                "not-an-email",
                "Password123",
                DateTime.UtcNow));
    }

    [Test]
    public void Create_WithEmptyPassword_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                "test@example.com",
                "",
                DateTime.UtcNow));
    }

    [Test]
    public void CheckPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        User user = User.Create(
            "test@example.com",
            "Password123",
            DateTime.UtcNow);

        // Act
        bool result = user.CheckPassword("Password123");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        User user = User.Create(
            "test@example.com",
            "Password123",
            DateTime.UtcNow);

        // Act
        bool result = user.CheckPassword("WrongPassword");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void SetEmail_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        User user = User.Create(
            "test@example.com",
            "Password123",
            DateTime.UtcNow);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            user.SetEmail("invalid."));
    }
    
    [Test]
    public void SetEmail_ShouldTrimWhitespace()
    {
        var user = User.Create("test@example.com", "Password123", DateTime.UtcNow);

        user.SetEmail("  new@example.com ");

        Assert.That(user.Email, Is.EqualTo("new@example.com"));
    }
    
    [Test]
    public void SetEmail_WithTrailingDot_ShouldThrow()
    {
        var user = User.Create("test@example.com", "Password123", DateTime.UtcNow);

        Assert.Throws<ArgumentException>(() =>
            user.SetEmail("test@example.com."));
    }
    
    [Test]
    public void PasswordHash_ShouldNotEqualPlainPassword()
    {
        var user = User.Create("test@example.com", "Password123", DateTime.UtcNow);

        Assert.That(user.PasswordHash, Is.Not.EqualTo("Password123"));
    }
    
    [Test]
    public void DifferentUsers_ShouldHaveDifferentSalts()
    {
        var user1 = User.Create("a@example.com", "Password123", DateTime.UtcNow);
        var user2 = User.Create("b@example.com", "Password123", DateTime.UtcNow);

        Assert.That(user1.Salt, Is.Not.EqualTo(user2.Salt));
    }
    
    [Test]
    public void CheckPassword_ShouldBeCaseSensitive()
    {
        var user = User.Create("test@example.com", "Password123", DateTime.UtcNow);

        Assert.That(user.CheckPassword("password123"), Is.False);
    }
    
    [Test]
    public void UserGuid_ShouldBeStable()
    {
        var user = User.Create("test@example.com", "Password123", DateTime.UtcNow);

        var guid = user.UserGuid;

        Assert.That(user.UserGuid, Is.EqualTo(guid));
    }
    
    [Test]
    public void Create_WithWhitespacePassword_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create("test@example.com", "   ", DateTime.UtcNow));
    }
}