using Microsoft.AspNetCore.Identity;
using TourPlanner.Application.Abstractions.Authentication;

namespace TourPlanner.Infrastructure.Authentication;

internal sealed class AspNetPasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new object(), password);
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            password);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}