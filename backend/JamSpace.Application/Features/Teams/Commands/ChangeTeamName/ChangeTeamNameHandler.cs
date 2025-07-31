using JamSpace.Application.Common.Common.Exceptions;
using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Features.Teams.Mappers;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamName;

public class ChangeTeamNameHandler : IRequestHandler<ChangeTeamNameCommand, TeamDto>
{
    private readonly ITeamRepository _repo;

    public ChangeTeamNameHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamDto> Handle(ChangeTeamNameCommand request, CancellationToken cancellationToken)
    {
        if (!await _repo.IsUserALeaderAsync(request.TeamId, request.RequestingUserId) &&
            !await _repo.IsUserAnAdminAsync(request.TeamId, request.RequestingUserId))
            throw new ForbiddenAccessException("Only team leaders and admins can change team names.");
        
        var team = await _repo.ChangeTeamNameAsync(request.TeamId,request.Name, cancellationToken);

        return TeamMapper.ToDto(team);
    }
}