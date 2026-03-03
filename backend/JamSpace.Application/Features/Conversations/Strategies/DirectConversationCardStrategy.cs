using JamSpace.Application.Features.Conversations.DTOs;
using JamSpace.Domain.Entities;
using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.Strategies;

public sealed class DirectConversationCardStrategy : IConversationCardStrategy
{
    public ChatType SupportedType => ChatType.Direct;
    

    public ConversationCardDto Map(Conversation conversation, Guid currentUserId, int unreadCount)
    {
        var other = conversation.Participants
            .FirstOrDefault(p => p.UserId != currentUserId);

        if (other is null)
            throw new InvalidOperationException("Direct conversation must have exactly two participants.");

        return new ConversationCardDto
        {
            Id = conversation.Id,
            Type = ChatType.Direct,
            DisplayName = other.User!.DisplayName,
            DisplayPictureUrl = other.User.ProfilePictureUrl,
            LastMessagePreview = conversation.LastMessagePreview,
            LastMessageAt = conversation.LastMessageAt,
            UnreadCount = unreadCount
        };
    }
}