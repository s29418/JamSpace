namespace JamSpace.Domain.Entities;

public class UserGenre
{
    public Guid GenreId { get; set; }
    public required Genre Genre { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
}