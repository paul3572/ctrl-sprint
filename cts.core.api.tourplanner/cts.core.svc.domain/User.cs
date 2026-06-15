using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace cts.core.svc.contracts;

public class User
{
    public User(
        string email,
        string passwordHash,
        string salt,
        DateTimeOffset createdAtUtc)
    {
        this.Email = email;
        this.PasswordHash = passwordHash;
        this.Salt = salt;
        this.CreatedAtUtc = createdAtUtc;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Key]
    public int UserId { get; private set; }
    
    public Guid UserGuid { get; private set; }
    
    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;
    
    public string Salt {get; private set;} = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    private List<Tour> tours = [];

    public virtual IReadOnlyList<Tour> Tours => tours;

    public static User Create(
        string email,
        string displayName,
        string passwordHash,
        DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email must not be empty.", nameof(email));
        }

        string trimmedEmail = email.Trim();

        if (!IsValidEmail(trimmedEmail))
        {
            throw new ArgumentException("Email is not valid.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name must not be empty.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash must not be empty.", nameof(passwordHash));
        }

        return new User(
            trimmedEmail,
            displayName.Trim(),
            passwordHash,
            createdAtUtc);
    }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}