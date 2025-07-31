using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Login;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<AuthResultDto>;