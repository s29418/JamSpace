using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Features.Teams.Mappers;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetMyTeams;

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