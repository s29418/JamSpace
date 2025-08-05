using JamSpace.Application.Features.TeamInvites.DTOs;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetMyPendingInvites;

public record GetMyPendingInvitesQuery(Guid UserId) : IRequest<List<TeamInviteDto>>;
