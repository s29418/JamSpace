using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
    Task RotateAsync(RefreshToken oldToken, string newToken, DateTime newExpiry, CancellationToken ct);
    Task RevokeAsync(string token, CancellationToken ct);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct);
}