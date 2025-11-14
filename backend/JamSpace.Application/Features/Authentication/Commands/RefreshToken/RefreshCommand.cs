using JamSpace.Application.Features.Authentication.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Refresh;

public record RefreshCommand(
    string RefreshTokenFromCookie) 
    : IRequest<AuthWithRefreshResult>;
