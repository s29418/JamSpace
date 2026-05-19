using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IProjectAudioVersionRepository
{
    Task<ProjectAudioVersion?> GetByIdAsync(Guid versionId, CancellationToken ct);
    Task<IReadOnlyList<ProjectAudioVersion>> GetByProjectIdAsync(Guid projectId, CancellationToken ct);
    Task AddAsync(ProjectAudioVersion version, CancellationToken ct);
    void Remove(ProjectAudioVersion version);
}
