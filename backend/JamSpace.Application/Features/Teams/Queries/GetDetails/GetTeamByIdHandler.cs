using JamSpace.Application.Common.Common.Exceptions;
using JamSpace.Application.Common.Features.Teams.Dtos;
using JamSpace.Application.Common.Features.Teams.Mappers;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Common.Features.Teams.Queries.GetDetails;

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

        return TeamMapper.ToDto(team!);
    }
}