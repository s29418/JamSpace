namespace JamSpace.Domain.Entities;

public class Message
{
    public required Guid Id { get; set; }
    
    public required Guid ConversationId { get; set; }
    public required Conversation Conversation { get; set; }
    
    public Guid SenderUserId { get; set; }
    public User? SenderUser { get; set; }
    
    public required string Content { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    
    public Guid? ReplyToMessageId { get; set; }
}