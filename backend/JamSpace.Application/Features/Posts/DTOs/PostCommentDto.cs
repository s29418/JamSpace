namespace JamSpace.Application.Features.Posts.DTOs;

public class PostCommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = null!;
    public string? UserProfilePictureUrl { get; set; }
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}