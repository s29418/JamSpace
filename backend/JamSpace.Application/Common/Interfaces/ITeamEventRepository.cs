using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface ITeamEventRepository
{
    Task<TeamEvent?> GetById(Guid eventId, CancellationToken ct);

    Task<IReadOnlyList<TeamEvent>> GetTeamEvents(Guid teamId, DateTimeOffset from, DateTimeOffset to, 
        CancellationToken ct);
    Task<bool> WasEventCreatedByUser(Guid eventId, Guid userId, CancellationToken ct);
    Task AddAsync(TeamEvent teamEvent, CancellationToken ct);
    void Remove(TeamEvent teamEvent);
}