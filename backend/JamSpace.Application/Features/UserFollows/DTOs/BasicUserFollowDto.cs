namespace JamSpace.Application.Features.UserFollows.DTOs;

public class BasicUserFollowDto
{
    public Guid FollowerId { get; set; }
    public Guid FolloweeId { get; set; }
}