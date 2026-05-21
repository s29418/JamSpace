using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid RecipientUserId { get; set; }
    public User RecipientUser { get; set; } = null!;

    public Guid? ActorUserId { get; set; }
    public User? ActorUser { get; set; }

    public NotificationType Type { get; set; }

    public required string Title { get; set; }
    public required string Message { get; set; }

    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    public Guid? ConversationId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? TeamInviteId { get; set; }
    public Guid? TeamEventId { get; set; }
    public Guid? PostId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ProjectNoteId { get; set; }
}
