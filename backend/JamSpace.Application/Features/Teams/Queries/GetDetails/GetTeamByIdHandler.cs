using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetDetails;

public class GetTeamByIdHandler : IRequestHandler<GetTeamByIdQuery, TeamDto>
{
    private readonly ITeamRepository _repo;

    public GetTeamByIdHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamDto> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var isMember = await _repo.IsUserInTeamAsync(request.TeamId, request.RequestingUserId);
        if (!isMember)
            throw new ForbiddenAccessException("You are not a member of this team.");

        var team = await _repo.GetTeamByIdAsync(request.TeamId);
        if (team is null)
            throw new NotFoundException("Team not found.");

        return TeamMapper.ToDto(team);
    }
}