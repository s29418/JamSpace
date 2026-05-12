using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ExternalOAuthStateRepository : IExternalOAuthStateRepository
{
    private readonly JamSpaceDbContext _db;

    public ExternalOAuthStateRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<ExternalOAuthState?> GetActiveByProviderAndStateAsync(
        ExternalMusicProvider provider,
        string state,
        DateTimeOffset now,
        CancellationToken ct)
    {
        return await _db.ExternalOAuthStates
            .FirstOrDefaultAsync(
                x => x.Provider == provider &&
                     x.State == state &&
                     x.ConsumedAt == null &&
                     x.ExpiresAt > now,
                ct);
    }

    public async Task AddAsync(ExternalOAuthState state, CancellationToken ct)
    {
        await _db.ExternalOAuthStates.AddAsync(state, ct);
    }

    public void MarkConsumed(ExternalOAuthState state, DateTimeOffset consumedAt)
    {
        state.ConsumedAt = consumedAt;
    }
}
