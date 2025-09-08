namespace JamSpace.Domain.Entities;

public class Genre
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; } 
    
    public ICollection<UserGenre> UserGenres { get; set; } = new List<UserGenre>();
}