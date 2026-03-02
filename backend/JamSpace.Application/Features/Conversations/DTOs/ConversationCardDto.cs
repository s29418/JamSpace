using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.DTOs;

public class ConversationCardDto
{
    public Guid Id { get; init; }
    public ChatType Type { get; init; }

    public string DisplayName { get; init; } = default!;
    public string? DisplayPictureUrl { get; init; }

    public string? LastMessagePreview { get; init; }
    public DateTimeOffset? LastMessageAt { get; init; }
    //int UnreadCount (maybe later)
}