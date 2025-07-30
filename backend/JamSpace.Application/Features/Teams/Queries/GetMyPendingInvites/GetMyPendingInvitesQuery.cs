using JamSpace.Application.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetMyPendingInvites;

public record GetMyPendingInvitesQuery(Guid UserId) : IRequest<List<TeamInviteDto>>;
