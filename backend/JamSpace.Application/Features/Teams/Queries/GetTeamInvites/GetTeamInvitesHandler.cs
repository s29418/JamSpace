using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetTeamInvites;

public class GetTeamInvitesHandler : IRequestHandler<GetTeamInvitesQuery, List<TeamInviteDto>>
{
    private readonly ITeamRepository _repo;

    public GetTeamInvitesHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TeamInviteDto>> Handle(GetTeamInvitesQuery request, CancellationToken ct)
    {
        if (!await _repo.IsUserInTeamAsync(request.TeamId, request.RequestingUserId))
            throw new ForbiddenAccessException("User is not in the team.");

        var invites = await _repo.GetTeamInvitesAsync(request.TeamId, request.RequestingUserId, ct);

        return invites
            .Select(TeamInviteMapper.ToDto)
            .ToList();
    }
}