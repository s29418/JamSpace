using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetMyPendingInvites;

public class GetMyPendingInvitesHandler : IRequestHandler<GetMyPendingInvitesQuery, List<TeamInviteDto>>
{
    private readonly ITeamInviteRepository _repo;

    public GetMyPendingInvitesHandler(ITeamInviteRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TeamInviteDto>> Handle(GetMyPendingInvitesQuery request, CancellationToken cancellationToken)
    {
        var invites = await _repo.GetMyPendingInvitesAsync(request.UserId, cancellationToken);

        return invites
            .Select(TeamInviteMapper.ToDto)
            .ToList();
    }
}
