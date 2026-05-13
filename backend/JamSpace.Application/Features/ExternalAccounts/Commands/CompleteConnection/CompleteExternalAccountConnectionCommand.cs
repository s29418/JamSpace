using JamSpace.Application.Features.ExternalAccounts.DTOs;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ExternalAccounts.Commands.CompleteConnection;

public sealed record CompleteExternalAccountConnectionCommand(
    ExternalMusicProvider Provider,
    string State,
    string Code
) : IRequest<UserExternalAccountDto>;
