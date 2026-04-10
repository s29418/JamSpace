using JamSpace.Domain.Enums;

namespace JamSpace.Domain.Entities;

public class PostMedia
{
    public Guid Id { get; set; }
    
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    
    public required string Url { get; set; }
    public MediaType MediaType { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Length { get; set; }
}