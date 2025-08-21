using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;       
    }

    public async Task<AuthResultDto> Handle(LoginUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct) 
                   ?? throw new ForbiddenAccessException("Invalid email or password.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new ForbiddenAccessException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResultDto(user.Id, user.UserName, user.Email, token);
    }
}