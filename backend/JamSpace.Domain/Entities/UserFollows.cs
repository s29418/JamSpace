namespace JamSpace.Domain.Entities;

public class UserFollows
{
    public Guid FollowerId { get; set; }
    public required User Follower { get; set; }
    public Guid FollowedId { get; set; }
    public required User Followed { get; set; }
    public DateTime FollowedAt { get; set; }
}