using cts.core.svc.application.Abstractions.Persistence;
using cts.core.svc.contracts;
using cts.core.svc.domain;
using Microsoft.EntityFrameworkCore;

namespace cts.core.svc.infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly TourPlannerDbContext _dbContext;

    public UserRepository(TourPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
}