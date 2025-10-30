using FluentValidation;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Authentication.Dtos;
using JamSpace.Domain.Entities;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicy _passwordPolicy;

    public RegisterUserHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, 
        IPasswordHasher passwordHasher, IPasswordPolicy passwordPolicy)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;       
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (existingUser != null)
            throw new ConflictException("Email already in use.");
        
        
        if (await _userRepository.GetUserIdByUsernameAsync(request.Username, ct) is not null)
            throw new ConflictException("Username already in use.");
        
        var (ok, errors) = _passwordPolicy.Validate(request.Password);
        if (!ok)
        {
            throw new ValidationException(errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Key, e.Value))
            );
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Username,
            DisplayName = request.Username,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _userRepository.AddAsync(user, ct);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResultDto(user.Id, user.UserName, user.Email, token);
    }
}