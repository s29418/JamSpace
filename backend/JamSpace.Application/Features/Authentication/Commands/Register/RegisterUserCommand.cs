using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string Username,
    string Password
) : IRequest<AuthResultDto>;