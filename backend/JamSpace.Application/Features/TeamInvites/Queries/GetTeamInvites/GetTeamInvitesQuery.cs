using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetTeamInvites;

public record GetTeamInvitesQuery(Guid TeamId, Guid RequestingUserId) : IRequest<List<TeamInviteDto>>;