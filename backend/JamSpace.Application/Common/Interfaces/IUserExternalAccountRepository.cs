using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserExternalAccountRepository
{
    Task<IReadOnlyList<UserExternalAccount>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
    Task<UserExternalAccount?> GetActiveByUserAndProviderAsync(
        Guid userId,
        ExternalMusicProvider provider,
        CancellationToken ct);
    Task<UserExternalAccount?> GetActiveByProviderExternalUserIdAsync(
        ExternalMusicProvider provider,
        string externalUserId,
        CancellationToken ct);
    Task AddAsync(UserExternalAccount account, CancellationToken ct);
}
