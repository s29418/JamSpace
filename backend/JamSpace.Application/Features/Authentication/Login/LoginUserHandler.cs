using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;
using Microsoft.Extensions.Options;

namespace JamSpace.Application.Features.Authentication.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwt;

    public LoginUserHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _jwt = jwtOptions.Value;
    }

    public async Task<AuthResultDto> Handle(LoginUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct)
                   ?? throw new ForbiddenAccessException("Invalid email or password.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new ForbiddenAccessException("Invalid email or password.");

        var access = _jwtTokenGenerator.GenerateAccessToken(user, user.TokenVersion, _jwt.AccessMinutes);

        return new AuthResultDto(user.Id, user.UserName, user.Email, access);
    }
}