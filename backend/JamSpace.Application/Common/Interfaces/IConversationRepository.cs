using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken ct);
    Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct);
    Task<Guid?> GetIdForDirectAsync(string directKey, CancellationToken ct);
    Task<Conversation?> GetForTeamAsync(Guid teamId, CancellationToken ct);
    Task AddAsync(Conversation conversation, CancellationToken ct);
    void Remove(Conversation conversation);
}