using DefaultNamespace;

namespace JamSpace.Application.Authentication;

using MediatR;

public record RegisterUserCommand(
    string Email,
    string Username,
    string Password
) : IRequest<AuthResultDto>;