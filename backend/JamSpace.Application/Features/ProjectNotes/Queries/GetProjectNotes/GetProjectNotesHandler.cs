using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Application.Features.ProjectNotes.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Queries.GetProjectNotes;

public class GetProjectNotesHandler : IRequestHandler<GetProjectNotesQuery, IReadOnlyList<ProjectNoteDto>>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;

    public GetProjectNotesHandler(
        IProjectRepository projects,
        IProjectAudioVersionRepository versions,
        IProjectNoteRepository notes,
        ITeamMemberRepository members)
    {
        _projects = projects;
        _versions = versions;
        _notes = notes;
        _members = members;
    }

    public async Task<IReadOnlyList<ProjectNoteDto>> Handle(GetProjectNotesQuery request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        if (request.AudioVersionId.HasValue)
        {
            var version = await _versions.GetByIdAsync(request.AudioVersionId.Value, ct);
            if (version is null || version.ProjectId != request.ProjectId)
                throw new NotFoundException("Audio version not found.");

            var versionNotes = await _notes.GetByProjectIdAndAudioVersionIdAsync(
                request.ProjectId,
                request.AudioVersionId.Value,
                ct);

            return versionNotes
                .Select(ProjectNoteMapper.ToDto)
                .ToList();
        }

        var notes = await _notes.GetByProjectIdAsync(request.ProjectId, ct);

        return notes
            .Select(ProjectNoteMapper.ToDto)
            .ToList();
    }
}
