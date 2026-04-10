namespace JamSpace.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    
    public string? Content { get; set; }
    
    public PostMedia? Media { get; set; }
    
    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}