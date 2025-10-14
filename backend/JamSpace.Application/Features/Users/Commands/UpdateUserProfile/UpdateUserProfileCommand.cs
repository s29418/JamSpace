using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Users.UpdateProfile;
using MediatR;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    bool SetBio,            string? Bio,
    bool SetProfilePicture, string? ProfilePictureUrl,
    bool SetLocation,       LocationDto? Location,
    bool SetEmail,          string? Email,
    byte[] RowVersion
) : IRequest<UserDto>;