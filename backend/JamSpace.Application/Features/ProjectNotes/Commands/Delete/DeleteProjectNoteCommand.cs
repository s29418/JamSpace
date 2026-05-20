using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Delete;

public record DeleteProjectNoteCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid NoteId,
    Guid RequestingUserId
) : IRequest;
