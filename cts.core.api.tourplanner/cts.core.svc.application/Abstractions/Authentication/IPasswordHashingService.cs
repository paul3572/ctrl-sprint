namespace cts.core.svc.application.Abstractions.Authentication;

public interface IPasswordHashingService
{
    string HashPassword(string password);

    bool VerifyPassword(string passwordHash, string password);
}