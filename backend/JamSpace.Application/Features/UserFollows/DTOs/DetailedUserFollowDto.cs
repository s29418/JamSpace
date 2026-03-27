namespace JamSpace.Application.Features.UserFollows.DTOs;

public class DetailedUserFollowDto : BasicUserFollowDto
{
    public string FollowerUsername { get; set; } = default!;
    public string FollowerDisplayName { get; set; } = default!;
    public string? FollowerPictureUrl { get; set; } 
    public bool IsFollowing { get; set; }
}