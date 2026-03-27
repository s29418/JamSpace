using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.Strategies;

public sealed class TeamConversationCardStrategy : IConversationCardStrategy
{
    public ChatType SupportedType => ChatType.Team;
    
    public ConversationCardDto Map(Conversation conversation, Guid currentUserId, int unreadCount)
    {
        if (conversation.Team is null)
            throw new InvalidOperationException("Team conversations must have Team loaded.");
        
        return new ConversationCardDto
        {
            Id = conversation.Id,
            Type = ChatType.Team,
            DisplayName = conversation.Team.Name,
            DisplayPictureUrl = conversation.Team.TeamPictureUrl,
            LastMessagePreview = conversation.LastMessagePreview,
            LastMessageAt = conversation.LastMessageAt,
            UnreadCount = unreadCount
        };
    }
}