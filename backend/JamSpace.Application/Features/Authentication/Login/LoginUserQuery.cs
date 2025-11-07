using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Login;

public record LoginUserQuery(
    string Email, 
    string Password,
    string? UserAgent,
    string? IpAddress) 
    : IRequest<AuthWithRefreshResult>;
