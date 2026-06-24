using cts.core.svc.contracts;
using cts.core.svc.domain;

namespace cts.core.svc.application.Abstractions.Persistence;

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