using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;

namespace JamSpace.Infrastructure.Services;

public class AuthSessionService : IAuthSessionService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;

    public AuthSessionService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _uow = uow;
    }

    public async Task LogoutAsync(Guid userId, string? refreshTokenFromCookie, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(refreshTokenFromCookie))
            await _refreshTokens.RevokeAsync(refreshTokenFromCookie, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task LogoutEverywhereAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");

        await _refreshTokens.RevokeAllForUserAsync(userId, ct);
        user.TokenVersion++;

        await _uow.SaveChangesAsync(ct);
    }
}