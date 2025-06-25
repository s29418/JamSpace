using DefaultNamespace;
using JamSpace.Application.Features.Teams.Dtos;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Application.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Teams.Create;

public class CreateTeamHandler : IRequestHandler<CreateTeamWithUserCommand, TeamDto>
{
    private readonly ITeamRepository _repo;

    public CreateTeamHandler(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeamDto> Handle(CreateTeamWithUserCommand request, CancellationToken cancellationToken)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Command.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedById = request.CreatorUserId,
            TeamPictureUrl = request.Command.TeamPictureUrl
        };

        var teamId = await _repo.CreateTeamAsync(team, request.CreatorUserId);

        var createdTeam = await _repo.GetTeamByIdAsync(teamId);
        return TeamMapper.ToDto(createdTeam!);
    }
}

