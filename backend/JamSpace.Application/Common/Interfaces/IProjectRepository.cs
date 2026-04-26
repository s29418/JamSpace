using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetByTeamId(Guid teamId, CancellationToken ct);
    Task AddAsync(Project project, CancellationToken ct);
}