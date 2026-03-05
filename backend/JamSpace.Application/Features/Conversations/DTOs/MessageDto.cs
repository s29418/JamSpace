namespace JamSpace.Application.Features.Conversations.DTOs;

public class MessageDto
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public Guid SenderUserId { get; init; }
    public string SenderUsername { get; init; } = default!;
    public string? SenderPictureUrl { get; init; }
    public string Content { get; init; } = default!;
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? ReplyToMessageId { get; init; }
}