namespace JamSpace.Application.Features.Conversations.DTOs;

public class ConversationReadDto
{
    public Guid ConversationId{ get; init; }
    public Guid UserId{ get; init; }
    public Guid? LastReadMessageId{ get; init; }
    public DateTimeOffset? LastReadAt { get; init; }
}