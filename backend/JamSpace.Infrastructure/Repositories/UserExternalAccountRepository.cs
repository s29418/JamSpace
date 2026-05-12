using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class UserExternalAccountRepository : IUserExternalAccountRepository
{
    private readonly JamSpaceDbContext _db;

    public UserExternalAccountRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UserExternalAccount>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserExternalAccounts
            .Where(x => x.UserId == userId && x.DisconnectedAt == null)
            .OrderBy(x => x.Provider)
            .ToListAsync(ct);
    }

    public async Task<UserExternalAccount?> GetActiveByUserAndProviderAsync(
        Guid userId,
        ExternalMusicProvider provider,
        CancellationToken ct)
    {
        return await _db.UserExternalAccounts
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.Provider == provider &&
                     x.DisconnectedAt == null,
                ct);
    }

    public async Task<UserExternalAccount?> GetActiveByProviderExternalUserIdAsync(
        ExternalMusicProvider provider,
        string externalUserId,
        CancellationToken ct)
    {
        return await _db.UserExternalAccounts
            .FirstOrDefaultAsync(
                x => x.Provider == provider &&
                     x.ExternalUserId == externalUserId &&
                     x.DisconnectedAt == null,
                ct);
    }

    public async Task AddAsync(UserExternalAccount account, CancellationToken ct)
    {
        await _db.UserExternalAccounts.AddAsync(account, ct);
    }
}
