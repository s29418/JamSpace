namespace JamSpace.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    
    public string? Content { get; set; }
    
    public PostMedia? Media { get; set; }
    
    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    // repost
    public Guid? OriginalPostId { get; set; }
    public Post? OriginalPost { get; set; }

    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public ICollection<Post> Reposts { get; set; } = new List<Post>();
}