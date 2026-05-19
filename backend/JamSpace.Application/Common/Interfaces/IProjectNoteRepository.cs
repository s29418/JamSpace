using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IProjectNoteRepository
{
    Task<IReadOnlyList<ProjectNote>> GetByAudioVersionIdAsync(Guid audioVersionId, CancellationToken ct);
}
