using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.AcceptTeamInvite;

public record AcceptTeamInviteCommand(Guid InviteId, Guid UserId) : IRequest<TeamInviteDto>;
