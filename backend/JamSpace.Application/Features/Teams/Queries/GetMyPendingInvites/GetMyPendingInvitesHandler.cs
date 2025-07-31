using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetMyPendingInvites;

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

        return invites.Select(i => new TeamInviteDto
        {
            Id = i.Id,
            TeamId = i.TeamId,
            TeamName = i.Team.Name,
            TeamPictureUrl = i.Team.TeamPictureUrl,
            CreatedAt = i.CreatedAt,
            InvitedByUserName = i.InvitedByUser.UserName
        }).ToList();
    }
}
