namespace JamSpace.Domain.Entities;

public class UserGenre
{
    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}