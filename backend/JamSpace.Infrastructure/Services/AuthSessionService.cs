using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;

namespace JamSpace.Infrastructure.Services;

public class AuthSessionService : IAuthSessionService
{
    private readonly IUnitOfWork _uow;

    public AuthSessionService(IUnitOfWork uow) => _uow = uow;

    public async Task LogoutAsync(Guid userId, string? refreshTokenFromCookie, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(refreshTokenFromCookie))
            await _uow.RefreshTokens.RevokeAsync(refreshTokenFromCookie, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task LogoutEverywhereAsync(Guid userId, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");

        await _uow.RefreshTokens.RevokeAllForUserAsync(userId, ct);
        user.TokenVersion++; 

        await _uow.SaveChangesAsync(ct);
    }
}
