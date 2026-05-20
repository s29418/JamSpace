using JamSpace.Application.Features.ProjectNotes.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Reopen;

public record ReopenProjectNoteCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid NoteId,
    Guid RequestingUserId
) : IRequest<ProjectNoteDto>;
