using JamSpace.Application.Features.ProjectNotes.DTOs;
using MediatR;

namespace JamSpace.Application.Features.ProjectNotes.Queries.GetProjectNotes;

public record GetProjectNotesQuery(
    Guid TeamId,
    Guid ProjectId,
    Guid RequestingUserId,
    Guid? AudioVersionId
) : IRequest<IReadOnlyList<ProjectNoteDto>>;
