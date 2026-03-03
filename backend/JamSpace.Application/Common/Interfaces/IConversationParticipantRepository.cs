using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IConversationParticipantRepository
{
    Task<ConversationParticipant?> GetAsync(Guid teamId, Guid userId, CancellationToken ct);
    Task AddAsync(ConversationParticipant conversationParticipant, CancellationToken ct);
    void Remove(ConversationParticipant conversationParticipant);
}