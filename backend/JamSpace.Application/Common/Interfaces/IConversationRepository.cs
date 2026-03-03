using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken ct);
    Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct);
    Task<Guid?> GetIdForDirect(string directKey, CancellationToken ct);
    Task AddAsync(Conversation conversation, CancellationToken ct);
}