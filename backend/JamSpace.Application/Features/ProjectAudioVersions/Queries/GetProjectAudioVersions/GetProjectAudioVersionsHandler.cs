using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ProjectAudioVersions.DTOs;
using JamSpace.Application.Features.ProjectAudioVersions.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectAudioVersions.Queries.GetProjectAudioVersions;

public class GetProjectAudioVersionsHandler
    : IRequestHandler<GetProjectAudioVersionsQuery, IReadOnlyList<ProjectAudioVersionDto>>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly ITeamMemberRepository _members;

    public GetProjectAudioVersionsHandler(
        IProjectRepository projects,
        IProjectAudioVersionRepository versions,
        ITeamMemberRepository members)
    {
        _projects = projects;
        _versions = versions;
        _members = members;
    }

    public async Task<IReadOnlyList<ProjectAudioVersionDto>> Handle(
        GetProjectAudioVersionsQuery request,
        CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        var versions = await _versions.GetByProjectIdAsync(request.ProjectId, ct);

        return versions
            .Select(ProjectAudioVersionMapper.ToDto)
            .ToList();
    }
}
