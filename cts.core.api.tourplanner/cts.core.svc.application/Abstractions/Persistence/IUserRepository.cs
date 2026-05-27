using TourPlanner.Domain.Users;

namespace TourPlanner.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}