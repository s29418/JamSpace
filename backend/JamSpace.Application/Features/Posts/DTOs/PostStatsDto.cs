namespace JamSpace.Application.Features.Posts.DTOs;

public class PostStatsDto
{
    public int LikeCount { get; init; }
    public int CommentCount { get; init; }
    public int RepostCount { get; init; }
    public bool IsLikedByCurrentUser { get; init; }
    public bool IsRepostedByCurrentUser { get; init; }
}