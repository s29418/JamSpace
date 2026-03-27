namespace JamSpace.Application.Features.UserGenres.DTOs;

public class UserGenreDto
{
    public Guid GenreId { get; set; }
    public string GenreName { get; set; } = default!;
    
    public UserGenreDto(Guid id, string name)
    {
        GenreId = id;
        GenreName = name;
    }

    public UserGenreDto()
    {
    }
}