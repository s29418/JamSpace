using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.Disconnect;

public sealed record DisconnectExternalAccountCommand(
    Guid UserId,
    ExternalMusicProvider Provider
) : IRequest;
