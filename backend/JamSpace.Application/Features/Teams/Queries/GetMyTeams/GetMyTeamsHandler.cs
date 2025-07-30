using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Queries.GetMyTeams;

public class GetMyTeamsHandler : IRequestHandler<GetMyTeamsQuery, List<TeamDto>>
{
    private readonly ITeamRepository _repo;

    public GetMyTeamsHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TeamDto>> Handle(GetMyTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await _repo.GetTeamsByUserIdAsync(request.UserId, cancellationToken);
        return teams
            .Select(TeamMapper.ToDto)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }
}