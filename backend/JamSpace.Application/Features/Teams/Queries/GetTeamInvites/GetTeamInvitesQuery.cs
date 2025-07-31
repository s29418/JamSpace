using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetTeamInvites;

public record GetTeamInvitesQuery(Guid TeamId, Guid RequestingUserId) : IRequest<List<TeamInviteDto>>;