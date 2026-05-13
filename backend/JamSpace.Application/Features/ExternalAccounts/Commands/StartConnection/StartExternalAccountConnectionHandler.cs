using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.StartConnection;

public class StartExternalAccountConnectionHandler
    : IRequestHandler<StartExternalAccountConnectionCommand, ExternalAuthUrl>
{
    private static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(10);

    private readonly IUserRepository _users;
    private readonly IUserExternalAccountRepository _externalAccounts;
    private readonly IExternalOAuthStateRepository _oauthStates;
    private readonly IMusicPlatformAuthClientResolver _clientResolver;
    private readonly IOAuthPkceGenerator _pkceGenerator;
    private readonly ITokenProtector _tokenProtector;
    private readonly IUnitOfWork _uow;

    public StartExternalAccountConnectionHandler(
        IUserRepository users,
        IUserExternalAccountRepository externalAccounts,
        IExternalOAuthStateRepository oauthStates,
        IMusicPlatformAuthClientResolver clientResolver,
        IOAuthPkceGenerator pkceGenerator,
        ITokenProtector tokenProtector,
        IUnitOfWork uow)
    {
        _users = users;
        _externalAccounts = externalAccounts;
        _oauthStates = oauthStates;
        _clientResolver = clientResolver;
        _pkceGenerator = pkceGenerator;
        _tokenProtector = tokenProtector;
        _uow = uow;
    }

    public async Task<ExternalAuthUrl> Handle(
        StartExternalAccountConnectionCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _users.ExistsAsync(request.UserId, cancellationToken))
            throw new NotFoundException("User not found.");

        var existingAccount = await _externalAccounts.GetActiveByUserAndProviderAsync(
            request.UserId,
            request.Provider,
            cancellationToken);

        if (existingAccount is not null)
            throw new ConflictException($"{request.Provider} account is already connected.");

        var client = _clientResolver.Resolve(request.Provider);
        var pkce = _pkceGenerator.Generate();
        var now = DateTimeOffset.UtcNow;

        var oauthState = new ExternalOAuthState
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Provider = request.Provider,
            State = pkce.State,
            CodeVerifier = _tokenProtector.Protect(pkce.CodeVerifier),
            ReturnUrl = string.IsNullOrWhiteSpace(request.ReturnUrl)
                ? null
                : request.ReturnUrl.Trim(),
            CreatedAt = now,
            ExpiresAt = now.Add(StateLifetime)
        };

        await _oauthStates.AddAsync(oauthState, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return client.BuildAuthorizationUrl(pkce.State, pkce.CodeChallenge);
    }
}
