using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class Conversation
{
    public required Guid Id { get; set; }
    public required ChatType Type { get; set; }
    
    public Guid? TeamId { get; set; }
    public string? DirectKey { get; set; }
    
    public DateTimeOffset? LastMessageAt { get; set; }
    public string? LastMessagePreview { get; set; }
    public Guid? LastMessageId { get; set; }
}