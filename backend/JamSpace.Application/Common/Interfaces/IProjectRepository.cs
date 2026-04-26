using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken ct);
    Task<IReadOnlyList<Project>> GetByTeamIdAsync(Guid teamId, CancellationToken ct);
    Task AddAsync(Project project, CancellationToken ct);
}