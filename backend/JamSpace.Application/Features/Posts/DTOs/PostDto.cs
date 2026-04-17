namespace JamSpace.Application.Features.Posts.DTOs;

public class PostDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    
    public Guid AuthorId { get; set; }
    public string? AuthorDisplayName { get; set; } 
    public string? AuthorAvatarUrl { get; set; }
    
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int RepostCount { get; set; }
    
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsRepostedByCurrentUser { get; set; }
    
    public PostDto? OriginalPost { get; set; }

    public IEnumerable<PostCommentDto> Comments { get; set; } = [];
}