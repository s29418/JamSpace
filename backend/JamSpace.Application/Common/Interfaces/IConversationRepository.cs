using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IConversationRepository
{
    Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct);
    Task AddAsync(Conversation conversation, CancellationToken ct);
}