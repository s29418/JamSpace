using JamSpace.Application.Common.Features.Teams.Dtos;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetMyPendingInvites;

public record GetMyPendingInvitesQuery(Guid UserId) : IRequest<List<TeamInviteDto>>;
