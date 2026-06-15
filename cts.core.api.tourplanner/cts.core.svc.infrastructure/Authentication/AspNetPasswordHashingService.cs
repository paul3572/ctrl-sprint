using cts.core.svc.application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace cts.core.svc.infrastructure.Authentication;

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