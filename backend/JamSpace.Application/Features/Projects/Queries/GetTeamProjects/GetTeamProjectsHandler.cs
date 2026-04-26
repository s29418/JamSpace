using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.Projects.DTOs;
using JamSpace.Application.Features.Projects.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.Projects.Queries.GetTeamProjects;

public class GetTeamProjectsHandler : IRequestHandler<GetTeamProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projects;
    private readonly ITeamMemberRepository _members;

    public GetTeamProjectsHandler(IProjectRepository projects, ITeamMemberRepository members)
    {
        _projects = projects;
        _members = members;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetTeamProjectsQuery request, CancellationToken ct)
    {
        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        
        var projects = await _projects.GetByTeamIdAsync(request.TeamId, ct);

        return projects
            .Select(ProjectMapper.ToDto)
            .ToList();
    }
}