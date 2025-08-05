using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.TeamInvites.DTOs;
using JamSpace.Application.Features.TeamInvites.Mappers;
using MediatR;

namespace JamSpace.Application.Features.TeamInvites.Commands.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamInviteRepository _repo;

    public AcceptTeamInviteHandler(ITeamInviteRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamInviteDto> Handle(AcceptTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repo.AcceptInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
