using JamSpace.Application.Features.ProjectNotes.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Complete;

public record CompleteProjectNoteCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid NoteId,
    Guid RequestingUserId
) : IRequest<ProjectNoteDto>;
