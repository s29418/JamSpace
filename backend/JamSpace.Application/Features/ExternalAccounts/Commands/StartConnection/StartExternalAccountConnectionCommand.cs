using JamSpace.Application.Common.Models;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.StartConnection;

public sealed record StartExternalAccountConnectionCommand(
    Guid UserId,
    ExternalMusicProvider Provider,
    string? ReturnUrl
) : IRequest<ExternalAuthUrl>;
