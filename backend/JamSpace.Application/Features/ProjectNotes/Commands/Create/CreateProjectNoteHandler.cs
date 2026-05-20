using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Application.Features.ProjectNotes.Mappers;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Create;

public class CreateProjectNoteHandler : IRequestHandler<CreateProjectNoteCommand, ProjectNoteDto>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectAudioVersionRepository _versions;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public CreateProjectNoteHandler(
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

    public async Task<ProjectNoteDto> Handle(CreateProjectNoteCommand request, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(request.ProjectId, ct);
        if (project is null || project.TeamId != request.TeamId)
            throw new NotFoundException("Project not found.");

        var isTeamMember =
            await _members.HasRequiredRoleAsync(request.TeamId, request.RequestingUserId, FunctionalRole.Member, ct);

        if (!isTeamMember)
            throw new ForbiddenAccessException("You are not part of this team.");

        ProjectAudioVersion? audioVersion = null;
        if (request.AudioVersionId.HasValue)
        {
            audioVersion = await _versions.GetByIdAsync(request.AudioVersionId.Value, ct);
            if (audioVersion is null || audioVersion.ProjectId != request.ProjectId)
                throw new NotFoundException("Audio version not found.");
        }

        var now = DateTimeOffset.UtcNow;
        var noteId = Guid.NewGuid();
        var note = new ProjectNote
        {
            Id = noteId,
            ProjectId = request.ProjectId,
            AudioVersionId = request.AudioVersionId,
            AudioVersionNameSnapshot = audioVersion?.Name,
            CreatedById = request.RequestingUserId,
            Content = request.Content.Trim(),
            StartTimeSeconds = request.StartTimeSeconds,
            EndTimeSeconds = request.EndTimeSeconds,
            Status = ProjectNoteStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _notes.AddAsync(note, ct);
        await _uow.SaveChangesAsync(ct);

        var savedNote = await _notes.GetByIdAsync(noteId, ct);
        return ProjectNoteMapper.ToDto(savedNote!);
    }
}
