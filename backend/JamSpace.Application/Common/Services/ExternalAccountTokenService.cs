using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Services;

public class ExternalAccountTokenService : IExternalAccountTokenService
{
    private static readonly TimeSpan RefreshSkew = TimeSpan.FromMinutes(2);

    private readonly IMusicPlatformAuthClientResolver _clientResolver;
    private readonly IUserExternalAccountRepository _externalAccounts;
    private readonly ITokenProtector _tokenProtector;
    private readonly IUnitOfWork _uow;

    public ExternalAccountTokenService(
        IMusicPlatformAuthClientResolver clientResolver,
        IUserExternalAccountRepository externalAccounts,
        ITokenProtector tokenProtector,
        IUnitOfWork uow)
    {
        _clientResolver = clientResolver;
        _externalAccounts = externalAccounts;
        _tokenProtector = tokenProtector;
        _uow = uow;
    }

    public async Task<string> GetValidAccessTokenAsync(UserExternalAccount account, CancellationToken ct)
    {
        if (account.DisconnectedAt is not null)
            throw new InvalidOperationException("External account is disconnected.");

        if (!ShouldRefresh(account))
            return _tokenProtector.Unprotect(account.AccessToken);

        if (string.IsNullOrWhiteSpace(account.RefreshToken))
            throw new InvalidOperationException("External account does not have a refresh token.");

        var client = _clientResolver.Resolve(account.Provider);
        var refreshToken = _tokenProtector.Unprotect(account.RefreshToken);
        var refreshedToken = await client.RefreshTokenAsync(refreshToken, ct);

        var protectedAccessToken = _tokenProtector.Protect(refreshedToken.AccessToken);
        var protectedRefreshToken = refreshedToken.RefreshToken is null
            ? null
            : _tokenProtector.Protect(refreshedToken.RefreshToken);

        _externalAccounts.UpdateTokens(
            account,
            protectedAccessToken,
            protectedRefreshToken,
            refreshedToken.ExpiresAt,
            refreshedToken.Scope ?? account.Scopes,
            DateTimeOffset.UtcNow);

        await _uow.SaveChangesAsync(ct);

        return refreshedToken.AccessToken;
    }

    private static bool ShouldRefresh(UserExternalAccount account)
    {
        return account.TokenExpiresAt.HasValue &&
               account.TokenExpiresAt.Value <= DateTimeOffset.UtcNow.Add(RefreshSkew);
    }
}
