namespace JamSpace.Application.Features.UserFollows.DTOs;

public class UserFollowDto
{
    public Guid FollowerId { get; set; }
    public Guid FolloweeId { get; set; }
}