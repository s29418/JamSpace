using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Application.Features.ProjectNotes.DTOs;
using JamSpace.Application.Features.ProjectNotes.Mappers;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Reopen;

public class ReopenProjectNoteHandler : IRequestHandler<ReopenProjectNoteCommand, ProjectNoteDto>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public ReopenProjectNoteHandler(
        IProjectRepository projects,
        IProjectNoteRepository notes,
        ITeamMemberRepository members,
        IUnitOfWork uow)
    {
        _projects = projects;
        _notes = notes;
        _members = members;
        _uow = uow;
    }

    public async Task<ProjectNoteDto> Handle(ReopenProjectNoteCommand request, CancellationToken ct)
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

        note.Status = ProjectNoteStatus.Active;
        note.CompletedById = null;
        note.CompletedAt = null;
        note.UpdatedAt = DateTimeOffset.UtcNow;

        await _uow.SaveChangesAsync(ct);

        var savedNote = await _notes.GetByIdAsync(request.NoteId, ct);
        var musicalRoles = await GetMusicalRolesAsync(request.TeamId, ct);
        return ProjectNoteMapper.ToDto(savedNote!, musicalRoles);
    }

    private async Task<IReadOnlyDictionary<Guid, string?>> GetMusicalRolesAsync(Guid teamId, CancellationToken ct)
    {
        var members = await _members.GetByTeamIdAsync(teamId, ct);
        return members
            .GroupBy(member => member.UserId)
            .ToDictionary(group => group.Key, group => group.First().MusicalRole);
    }
}
