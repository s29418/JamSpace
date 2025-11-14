using System.ComponentModel.DataAnnotations;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Settings;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace JamSpace.Application.Features.Authentication.Commands.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwt;

    public RegisterUserHandler(
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

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (existing != null)
            throw new ValidationException("Email is already taken.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Username,
            DisplayName = request.Username,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            TokenVersion = 0
        };

        await _userRepository.AddAsync(user, ct);

        var access = _jwtTokenGenerator.GenerateAccessToken(user, user.TokenVersion, _jwt.AccessMinutes);

        return new AuthResultDto(user.Id, user.UserName, user.Email, access);
    }
}