using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.AcceptTeamInvite;

public class AcceptTeamInviteHandler : IRequestHandler<AcceptTeamInviteCommand, TeamInviteDto>
{
    private readonly ITeamRepository _repo;

    public AcceptTeamInviteHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamInviteDto> Handle(AcceptTeamInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repo.AcceptInviteAsync(request.InviteId, request.UserId, cancellationToken);
        return TeamInviteMapper.ToDto(invite);
    }
}
