namespace JamSpace.Application.Features.Conversations.DTOs;

public class ConversationUpdatedDto
{
    public Guid ConversationId { get; init; } 
    public DateTimeOffset? LastMessageAt { get; init; }
    public string? LastMessagePreview { get; init; }
    public int UnreadCount { get; init; }
} 