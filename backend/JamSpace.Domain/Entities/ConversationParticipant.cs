namespace JamSpace.Domain.Entities;

public class ConversationParticipant
{
    public required Guid ConversationId { get; set; }
    public required Conversation Conversation { get; set; }
    
    public required Guid UserId { get; set; }
    public required User User { get; set; }
    
    public string? Role { get; set; }
    public Guid? LastReadMessageId { get; set; }
}