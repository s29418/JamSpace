using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Queries.GetTeamInvites;

public class GetTeamInvitesHandler : IRequestHandler<GetTeamInvitesQuery, List<TeamInviteDto>>
{
    private readonly ITeamInviteRepository _teamInviteRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public GetTeamInvitesHandler(ITeamInviteRepository teamInviteRepository, ITeamMemberRepository teamMemberRepository)
    {
        _teamInviteRepository = teamInviteRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<List<TeamInviteDto>> Handle(GetTeamInvitesQuery request, CancellationToken ct)
    {
        if (!await _teamMemberRepository.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct))
            throw new ForbiddenAccessException("User is not in the team.");

        var invites = await _teamInviteRepository
            .GetTeamInvitesAsync(request.TeamId, request.RequestingUserId, ct);

        return invites
            .Select(TeamInviteMapper.ToDto)
            .ToList();
    }
}