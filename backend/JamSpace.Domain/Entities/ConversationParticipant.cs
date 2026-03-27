namespace JamSpace.Domain.Entities;

public class ConversationParticipant
{
    public required Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public string? Role { get; set; }
    public Guid? LastReadMessageId { get; set; }
    public DateTimeOffset? LastReadAt { get; set; }
}