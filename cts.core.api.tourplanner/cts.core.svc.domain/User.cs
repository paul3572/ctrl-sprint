using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace cts.core.svc.contracts;

public class User
{
    public User(
        string email,
        string passwordHash,
        DateTime createdAtUtc)
    {
        this.UserGuid = Guid.NewGuid();
        this.SetEmail(email);
        this.SetPassword(passwordHash);
        this.CreatedAtUtc = createdAtUtc;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Key]
    public int UserId { get; private set; }
    
    public Guid UserGuid { get; private set; }
    
    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;
    
    public string Salt {get; private set;} = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    private List<Tour> tours = [];

    public virtual IReadOnlyList<Tour> Tours => tours;

    public static User Create(
        string email,
        string passwordHash,
        DateTime createdAtUtc)
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

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash must not be empty.", nameof(passwordHash));
        }

        return new User(
            trimmedEmail,
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
    
    // Hint for the compiler that we initialize some properties in this method.
    [MemberNotNull(nameof(Salt), nameof(PasswordHash))]
    public void SetPassword(string password)
    {
        this.Salt = this.GenerateRandomSalt();
        this.PasswordHash = this.CalculateHash(password, Salt);
    }
    
    /// <summary>
    /// Generates a random number with the given length of bits.
    /// </summary>
    /// <param name="length">Default: 128 bits (16 Bytes)</param>
    /// <returns>A base64 encoded string from the byte array.</returns>
    private string GenerateRandomSalt(int length = 128)
    {
        byte[] salt = new byte[length / 8];
        using (System.Security.Cryptography.RandomNumberGenerator rnd =
               System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rnd.GetBytes(salt);
        }

        return Convert.ToBase64String(salt);
    }
    
    /// <summary>
    /// Calculates a HMACSHA256 hash value with a given salt.
    /// </summary>
    /// <returns>Base64 encoded hash.</returns>
    private string CalculateHash(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        System.Security.Cryptography.HMACSHA256 myHash =
            new System.Security.Cryptography.HMACSHA256(saltBytes);

        byte[] hashedData = myHash.ComputeHash(passwordBytes);

        // Das Bytearray wird als Hexstring zurückgegeben.
        return Convert.ToBase64String(hashedData);
    }
    
    public void SetEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            throw new ArgumentException("Invalid Email");
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address == trimmedEmail)
                this.Email = trimmedEmail;
        }
        catch
        {
            throw new ArgumentException("Invalid Email");
        }
    }
}