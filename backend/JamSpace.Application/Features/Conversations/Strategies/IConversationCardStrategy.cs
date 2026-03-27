using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.Strategies;

public interface IConversationCardStrategy
{
    ChatType SupportedType { get; }

    ConversationCardDto Map(
        Conversation conversation,
        Guid currentUserId,
        int unreadCount);
}