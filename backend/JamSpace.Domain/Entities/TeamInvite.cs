namespace DefaultNamespace;

public class TeamInvite
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid InvitedUserId { get; set; }
    public User InvitedUser { get; set; } = null!;

    public Guid InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    public string Status { get; set; } = "pending"; // pending / accepted / rejected
    public DateTime CreatedAt { get; set; }
}