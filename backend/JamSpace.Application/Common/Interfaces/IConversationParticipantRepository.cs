using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IConversationParticipantRepository
{
    Task AddAsync(ConversationParticipant conversationParticipant, CancellationToken ct);
}