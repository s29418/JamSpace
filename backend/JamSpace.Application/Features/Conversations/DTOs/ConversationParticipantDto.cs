namespace JamSpace.Application.Features.Conversations.DTOs;

public class ConversationParticipantDto
{
    public Guid UserId { get; init; }

    public string DisplayName { get; init; } = default!;
    public string? AvatarUrl { get; init; }
    
    public Guid? LastReadMessageId { get; init; }
    public DateTimeOffset? LastReadAt { get; init; }
}