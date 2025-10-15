using JamSpace.Application.Features.Users.DTOs;

namespace JamSpace.API.Requests;

public record UpdateUserProfileRequest(
    bool SetBio,
    string? Bio,
    bool SetProfilePicture,
    string? ProfilePictureUrl,
    bool SetLocation,
    LocationDto? Location,
    bool SetEmail,
    string? Email
    );