using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IProjectNoteRepository
{
    Task<ProjectNote?> GetByIdAsync(Guid noteId, CancellationToken ct);
    Task<IReadOnlyList<ProjectNote>> GetByProjectIdAsync(Guid projectId, CancellationToken ct);
    Task<IReadOnlyList<ProjectNote>> GetByProjectIdAndAudioVersionIdAsync(
        Guid projectId,
        Guid audioVersionId,
        CancellationToken ct);
    Task<IReadOnlyList<ProjectNote>> GetByAudioVersionIdAsync(Guid audioVersionId, CancellationToken ct);
    Task AddAsync(ProjectNote note, CancellationToken ct);
    void Remove(ProjectNote note);
}
