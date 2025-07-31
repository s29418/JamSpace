using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetMyPendingInvites;

public class GetMyPendingInvitesHandler : IRequestHandler<GetMyPendingInvitesQuery, List<TeamInviteDto>>
{
    private readonly ITeamRepository _repo;

    public GetMyPendingInvitesHandler(ITeamRepository repo)
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
