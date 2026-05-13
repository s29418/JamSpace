using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IPortfolioTrackRepository
{
    Task<PortfolioTrack?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PortfolioTrack?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<PortfolioTrack>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<int> GetNextDisplayOrderAsync(Guid userId, CancellationToken ct);
    Task AddAsync(PortfolioTrack track, CancellationToken ct);
    void SoftDelete(PortfolioTrack track, DateTimeOffset deletedAt);
}
