namespace JamSpace.Application.Common.Interfaces;

public interface IAuthSessionService
{
    Task LogoutAsync(Guid userId, string? refreshTokenFromCookie, CancellationToken ct);
    Task LogoutEverywhereAsync(Guid userId, CancellationToken ct);
}