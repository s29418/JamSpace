using JamSpace.Application.Features.Users.DTOs;

namespace JamSpace.API.Requests;

public class UpdateUserProfileRequest
{
    public bool SetDisplayName { get; set; }
    public string? DisplayName { get; set; }
    public bool SetBio { get; set; }
    public string? Bio { get; set; }
    public bool SetProfilePicture { get; set; }
    public bool SetLocation { get; set; }
    public LocationDto? Location { get; set; }
    public bool SetEmail { get; set; }
    public string? Email { get; set; }
    
    public IFormFile? File { get; set; }
}
