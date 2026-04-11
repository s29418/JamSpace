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
}