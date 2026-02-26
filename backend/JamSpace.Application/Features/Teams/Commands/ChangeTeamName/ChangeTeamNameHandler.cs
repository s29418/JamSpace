using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Teams.DTOs;
using JamSpace.Application.Features.Teams.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Teams.Commands.ChangeTeamName;

public class ChangeTeamNameHandler : IRequestHandler<ChangeTeamNameCommand, TeamDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITeamMemberRepository _teamMemberRepo;

    public ChangeTeamNameHandler(ITeamRepository teamRepository, ITeamMemberRepository teamMemberRepo)
    {
        _teamRepository = teamRepository;
        _teamMemberRepo = teamMemberRepo;
    }

    public async Task<TeamDto> Handle(ChangeTeamNameCommand request, CancellationToken ct)
    {
        if (!await _teamMemberRepo.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Admin, ct))
            throw new ForbiddenAccessException("Only team leaders and admins can change team names.");
        
        var team = await _teamRepository.ChangeTeamNameAsync(request.TeamId,request.Name, ct);

        return TeamMapper.ToDto(team, request.RequestingUserId);
    }
}