using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly TourPlannerDbContext _dbContext;

    public UserRepository(TourPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.NormalizedEmail == normalizedEmail,
                cancellationToken);
    }

    public Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AnyAsync(
                user => user.NormalizedEmail == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
}