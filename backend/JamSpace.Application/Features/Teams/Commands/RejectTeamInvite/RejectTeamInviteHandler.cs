using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.RejectTeamInvite;

public class RejectTeamInviteHandler : IRequestHandler<RejectTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamRepository _repo;

    public RejectTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamInviteDto> Handle(RejectTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repo.RejectInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
