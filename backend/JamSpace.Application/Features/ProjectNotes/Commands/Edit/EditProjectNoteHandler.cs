using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Application.Features.ProjectNotes.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Edit;

public class EditProjectNoteHandler : IRequestHandler<EditProjectNoteCommand, ProjectNoteDto>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public EditProjectNoteHandler(
        IProjectRepository projects,
        IProjectAudioVersionRepository versions,
        IProjectNoteRepository notes,
        ITeamMemberRepository members,
        IUnitOfWork uow)
    {
        _projects = projects;
        _versions = versions;
        _notes = notes;
        _members = members;
        _uow = uow;
    }

    public async Task<ProjectNoteDto> Handle(EditProjectNoteCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var note = await _notes.GetByIdAsync(request.NoteId, ct);
        if (note is null || note.ProjectId != request.ProjectId)
            throw new NotFoundException("Note not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        if (note.CreatedById != request.RequestingUserId)
            throw new ForbiddenAccessException("You can only edit notes created by yourself.");

        ProjectAudioVersion? audioVersion = null;
        if (request.AudioVersionId.HasValue)
        {
            audioVersion = await _versions.GetByIdAsync(request.AudioVersionId.Value, ct);
            if (audioVersion is null || audioVersion.ProjectId != request.ProjectId)
                throw new NotFoundException("Audio version not found.");
        }

        note.Content = request.Content.Trim();
        note.AudioVersionId = request.AudioVersionId;
        note.AudioVersionNameSnapshot = audioVersion?.Name;
        note.IsAudioVersionDeleted = false;
        note.StartTimeSeconds = request.StartTimeSeconds;
        note.EndTimeSeconds = request.EndTimeSeconds;
        note.UpdatedAt = DateTimeOffset.UtcNow;

        await _uow.SaveChangesAsync(ct);

        var savedNote = await _notes.GetByIdAsync(request.NoteId, ct);
        return ProjectNoteMapper.ToDto(savedNote!);
    }
}
