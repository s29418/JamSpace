using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamEventRepository
{
    Task<TeamEvent?> GetByIdAsync(Guid eventId, CancellationToken ct);

    Task<IReadOnlyList<TeamEvent>> GetTeamEventsAsync(Guid teamId, DateTimeOffset from, DateTimeOffset to, 
        CancellationToken ct);
    Task<bool> WasEventCreatedByUserAsync(Guid eventId, Guid userId, CancellationToken ct);
    Task AddAsync(TeamEvent teamEvent, CancellationToken ct);
    void Remove(TeamEvent teamEvent);
}