using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class TeamInvite
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid InvitedUserId { get; set; }
    public User InvitedUser { get; set; } = null!;

    public Guid InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public DateTime CreatedAt { get; set; }
}