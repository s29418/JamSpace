using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IMessageRepository
{
    Task<Dictionary<Guid, int>> GetUnreadCountsAsync(Guid userId, IReadOnlyList<Guid> conversationIds,
        CancellationToken ct);
    Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, DateTimeOffset? before, 
        int take, CancellationToken ct);
    Task AddAsync(Message message, CancellationToken ct);
}