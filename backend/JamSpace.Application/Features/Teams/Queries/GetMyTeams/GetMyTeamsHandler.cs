using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Mappers;
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
        var teams = await _repo.GetByUserIdAsync(request.UserId, cancellationToken);
        return teams
            .Select(t => TeamMapper.ToDto(t, request.UserId))
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }
}