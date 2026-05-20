using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Domain.Enums;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Delete;

public class DeleteProjectNoteHandler : IRequestHandler<DeleteProjectNoteCommand>
{
    private readonly IProjectRepository _projects;
    private readonly IProjectNoteRepository _notes;
    private readonly ITeamMemberRepository _members;
    private readonly IUnitOfWork _uow;

    public DeleteProjectNoteHandler(
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

    public async Task Handle(DeleteProjectNoteCommand request, CancellationToken ct)
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

        var canDelete = note.CreatedById == request.RequestingUserId ||
                        await _members.HasRequiredRoleAsync(
                            request.TeamId,
                            request.RequestingUserId,
                            FunctionalRole.Admin,
                            ct);

        if (!canDelete)
            throw new ForbiddenAccessException("You must be an admin to delete notes created by other users.");

        _notes.Remove(note);
        await _uow.SaveChangesAsync(ct);
    }
}
