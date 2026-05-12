using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Common.Interfaces;

public interface IExternalOAuthStateRepository
{
    Task<ExternalOAuthState?> GetActiveByProviderAndStateAsync(
        ExternalMusicProvider provider,
        string state,
        DateTimeOffset now,
        CancellationToken ct);

    Task AddAsync(ExternalOAuthState state, CancellationToken ct);
    void MarkConsumed(ExternalOAuthState state, DateTimeOffset consumedAt);
}
