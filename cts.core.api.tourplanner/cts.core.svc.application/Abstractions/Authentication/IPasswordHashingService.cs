namespace TourPlanner.Application.Abstractions.Authentication;

public interface IPasswordHashingService
{
    string HashPassword(string password);

    bool VerifyPassword(string passwordHash, string password);
}