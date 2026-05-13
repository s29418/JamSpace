using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.CompleteConnection;

public class CompleteExternalAccountConnectionHandler
    : IRequestHandler<CompleteExternalAccountConnectionCommand, UserExternalAccountDto>
{
    private readonly IExternalOAuthStateRepository _oauthStates;
    private readonly IUserExternalAccountRepository _externalAccounts;
    private readonly IMusicPlatformAuthClientResolver _clientResolver;
    private readonly ITokenProtector _tokenProtector;
    private readonly IUnitOfWork _uow;

    public CompleteExternalAccountConnectionHandler(
        IExternalOAuthStateRepository oauthStates,
        IUserExternalAccountRepository externalAccounts,
        IMusicPlatformAuthClientResolver clientResolver,
        ITokenProtector tokenProtector,
        IUnitOfWork uow)
    {
        _oauthStates = oauthStates;
        _externalAccounts = externalAccounts;
        _clientResolver = clientResolver;
        _tokenProtector = tokenProtector;
        _uow = uow;
    }

    public async Task<UserExternalAccountDto> Handle(
        CompleteExternalAccountConnectionCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var oauthState = await _oauthStates.GetActiveByProviderAndStateAsync(
            request.Provider,
            request.State,
            now,
            cancellationToken);

        if (oauthState is null)
            throw new InvalidOperationException("OAuth state is invalid or expired.");

        var existingUserAccount = await _externalAccounts.GetActiveByUserAndProviderAsync(
            oauthState.UserId,
            request.Provider,
            cancellationToken);

        if (existingUserAccount is not null)
            throw new ConflictException($"{request.Provider} account is already connected.");

        var client = _clientResolver.Resolve(request.Provider);
        var codeVerifier = _tokenProtector.Unprotect(oauthState.CodeVerifier);
        var token = await client.ExchangeCodeAsync(request.Code, codeVerifier, cancellationToken);
        var profile = await client.GetCurrentUserProfileAsync(token.AccessToken, cancellationToken);

        var existingExternalAccount = await _externalAccounts.GetActiveByProviderExternalUserIdAsync(
            request.Provider,
            profile.ExternalUserId,
            cancellationToken);

        if (existingExternalAccount is not null && existingExternalAccount.UserId != oauthState.UserId)
            throw new ConflictException($"{request.Provider} account is already connected to another JamSpace user.");

        var account = new UserExternalAccount
        {
            Id = Guid.NewGuid(),
            UserId = oauthState.UserId,
            Provider = request.Provider,
            ExternalUserId = profile.ExternalUserId,
            DisplayName = profile.DisplayName,
            ProfileUrl = profile.ProfileUrl,
            AvatarUrl = profile.AvatarUrl,
            AccessToken = _tokenProtector.Protect(token.AccessToken),
            RefreshToken = token.RefreshToken is null ? null : _tokenProtector.Protect(token.RefreshToken),
            TokenExpiresAt = token.ExpiresAt,
            Scopes = token.Scope,
            ConnectedAt = now,
            UpdatedAt = now
        };

        await _externalAccounts.AddAsync(account, cancellationToken);
        _oauthStates.MarkConsumed(oauthState, now);
        await _uow.SaveChangesAsync(cancellationToken);

        return new UserExternalAccountDto(
            account.Id,
            account.Provider.ToString(),
            account.ExternalUserId,
            account.DisplayName,
            account.ProfileUrl,
            account.AvatarUrl,
            account.ConnectedAt,
            account.UpdatedAt);
    }
}
