namespace JamSpace.Application.Features.User.DTOs;

public class UserDto
{
    public string Username { get; set; } = null!;
    public string? UserPictureUrl { get; set; }
    public string? Bio { get; set; }
    public LocationDto? Location { get; set; }
    
    public string? SpotifyUrl { get; set; }
    public string? SoundcloudUrl { get; set; }
}