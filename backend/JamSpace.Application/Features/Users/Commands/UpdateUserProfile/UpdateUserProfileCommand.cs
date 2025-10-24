using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    bool SetDisplayName,
    string? DisplayName,
    bool SetBio,
    string? Bio,
    bool SetProfilePicture,
    string? ProfilePictureUrl,
    bool SetLocation,
    LocationDto? Location,
    bool SetEmail,
    string? Email
) : IRequest<UserDto>;
