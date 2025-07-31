using DefaultNamespace;
using MediatR;

namespace JamSpace.Application.Common.Authentication;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<AuthResultDto>;