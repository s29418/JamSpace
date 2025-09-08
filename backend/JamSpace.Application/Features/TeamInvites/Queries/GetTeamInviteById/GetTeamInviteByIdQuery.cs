using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetTeamInviteById;

public record GetTeamInviteByIdQuery(Guid Id) : IRequest<TeamInviteDto>;