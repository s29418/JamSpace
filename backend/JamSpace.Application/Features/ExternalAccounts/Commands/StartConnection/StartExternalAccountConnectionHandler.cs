using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Common.Settings;
using JamSpace.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

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
    private readonly MusicPlatformOAuthSettings _settings;

    public StartExternalAccountConnectionHandler(
        IUserRepository users,
        IUserExternalAccountRepository externalAccounts,
        IExternalOAuthStateRepository oauthStates,
        IMusicPlatformAuthClientResolver clientResolver,
        IOAuthPkceGenerator pkceGenerator,
        ITokenProtector tokenProtector,
        IUnitOfWork uow,
        IOptions<MusicPlatformOAuthSettings> settings)
    {
        _users = users;
        _externalAccounts = externalAccounts;
        _oauthStates = oauthStates;
        _clientResolver = clientResolver;
        _pkceGenerator = pkceGenerator;
        _tokenProtector = tokenProtector;
        _uow = uow;
        _settings = settings.Value;
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
        var returnUrl = NormalizeReturnUrl(request.ReturnUrl);

        var oauthState = new ExternalOAuthState
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Provider = request.Provider,
            State = pkce.State,
            CodeVerifier = _tokenProtector.Protect(pkce.CodeVerifier),
            ReturnUrl = returnUrl,
            CreatedAt = now,
            ExpiresAt = now.Add(StateLifetime)
        };

        await _oauthStates.AddAsync(oauthState, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return client.BuildAuthorizationUrl(pkce.State, pkce.CodeChallenge);
    }

    private string? NormalizeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return null;

        var trimmed = returnUrl.Trim();

        if (Uri.TryCreate(trimmed, UriKind.Relative, out _) &&
            trimmed.StartsWith('/') &&
            !trimmed.StartsWith("//"))
            return trimmed;

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
            uri.Scheme is not ("http" or "https"))
            throw new InvalidOperationException("Return URL is invalid.");

        var origin = uri.IsDefaultPort
            ? $"{uri.Scheme}://{uri.Host}"
            : $"{uri.Scheme}://{uri.Host}:{uri.Port}";

        var allowedOrigins = _settings.AllowedReturnUrlOrigins
            .Select(x => x.Trim().TrimEnd('/'))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!allowedOrigins.Contains(origin))
            throw new InvalidOperationException("Return URL origin is not allowed.");

        return trimmed;
    }
}
