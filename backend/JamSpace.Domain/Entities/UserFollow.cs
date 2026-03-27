namespace JamSpace.Domain.Entities;

public class UserFollow
{
    public Guid FollowerId { get; set; }
    public User Follower { get; set; } = null!;
    public Guid FolloweeId { get; set; }
    public User Followee { get; set; } = null!;
    public DateTime FollowedAt { get; set; }
}