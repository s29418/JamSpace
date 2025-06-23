using DefaultNamespace;
using MediatR;

namespace JamSpace.Application.Authentication;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<AuthResultDto>;