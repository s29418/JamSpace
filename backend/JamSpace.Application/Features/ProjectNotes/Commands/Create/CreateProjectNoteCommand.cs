using JamSpace.Application.Features.ProjectNotes.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Commands.Create;

public record CreateProjectNoteCommand(
    Guid TeamId,
    Guid ProjectId,
    Guid RequestingUserId,
    string Content,
    Guid? AudioVersionId,
    int? StartTimeSeconds,
    int? EndTimeSeconds
) : IRequest<ProjectNoteDto>;
