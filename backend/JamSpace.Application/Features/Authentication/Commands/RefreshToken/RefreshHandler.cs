using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace JamSpace.Application.Features.Authentication.Refresh;

public class RefreshHandler : IRequestHandler<RefreshCommand, AuthWithRefreshResult>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUserRepository _userRepo;
    private readonly IJwtTokenGenerator _jwt;
    private readonly JwtSettings _jwtSettings;

    public RefreshHandler(
        IRefreshTokenRepository refreshRepo,
        IUserRepository userRepo,
        IJwtTokenGenerator jwt,
        IOptions<JwtSettings> jwtOptions)
    {
        _refreshRepo = refreshRepo;
        _userRepo = userRepo;
        _jwt = jwt;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthWithRefreshResult> Handle(RefreshCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshTokenFromCookie))
            throw new ForbiddenAccessException("No refresh token.");

        var existing = await _refreshRepo.GetByTokenAsync(request.RefreshTokenFromCookie, ct)
                       ?? throw new ForbiddenAccessException("Invalid refresh token.");

        if (!existing.IsActive)
            throw new ForbiddenAccessException("Refresh token inactive.");

        var user = await _userRepo.GetByIdAsync(existing.UserId, ct)
                   ?? throw new ForbiddenAccessException("User not found.");
        
        var newRefresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        await _refreshRepo.RotateAsync(existing, newRefresh, DateTime.UtcNow.AddDays(_jwtSettings.RefreshDays), ct);
        
        var newAccess = _jwt.GenerateAccessToken(user, user.TokenVersion, _jwtSettings.AccessMinutes);

        return new AuthWithRefreshResult(user.Id, user.UserName, user.Email, newAccess, newRefresh);
    }
}