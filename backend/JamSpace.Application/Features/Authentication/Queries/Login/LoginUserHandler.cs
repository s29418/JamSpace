using System.Security.Cryptography;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace JamSpace.Application.Features.Authentication.Queries.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, AuthWithRefreshResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IJwtTokenGenerator _jwt;
    private readonly JwtSettings _jwtSettings;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _uow;

    public LoginUserHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshRepo,
        IJwtTokenGenerator jwt,
        IOptions<JwtSettings> jwtOptions,
        IPasswordHasher passwordHasher, IUnitOfWork uow) 
    {
        _userRepository = userRepository;
        _refreshRepo = refreshRepo;
        _jwt = jwt;
        _jwtSettings = jwtOptions.Value;
        _passwordHasher = passwordHasher;
        _uow = uow;
    }

    public async Task<AuthWithRefreshResult> Handle(LoginUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct)
                   ?? throw new ForbiddenAccessException("Invalid email or password.");
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash)) 
            throw new ForbiddenAccessException("Invalid email or password.");


        var access = _jwt.GenerateAccessToken(user, user.TokenVersion, _jwtSettings.AccessMinutes);
        
        var refreshBytes = RandomNumberGenerator.GetBytes(64);
        var refresh = Convert.ToBase64String(refreshBytes);

        await _refreshRepo.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refresh,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshDays),
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new AuthWithRefreshResult(user.Id, user.UserName, user.Email, access, refresh);
    }
}
