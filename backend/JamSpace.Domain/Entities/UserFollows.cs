namespace JamSpace.Domain.Entities;

public class UserFollows
{
    public Guid FollowerId { get; set; }
    public User Follower { get; set; } = null!;
    public Guid FollowedId { get; set; }
    public User Followed { get; set; } = null!;
    public DateTime FollowedAt { get; set; }
}