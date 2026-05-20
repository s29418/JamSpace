using JamSpace.Application.Features.ProjectNotes.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Edit;

public record EditProjectNoteCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid NoteId,
    Guid RequestingUserId,
    string Content,
    Guid? AudioVersionId,
    int? StartTimeSeconds,
    int? EndTimeSeconds
) : IRequest<ProjectNoteDto>;
