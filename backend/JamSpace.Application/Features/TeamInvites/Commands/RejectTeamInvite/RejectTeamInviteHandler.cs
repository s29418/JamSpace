using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.RejectTeamInvite;

public class RejectTeamInviteHandler : IRequestHandler<RejectTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _repo;

    public RejectTeamInviteHandler(ITeamInviteRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamInviteDto> Handle(RejectTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repo.RejectInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
