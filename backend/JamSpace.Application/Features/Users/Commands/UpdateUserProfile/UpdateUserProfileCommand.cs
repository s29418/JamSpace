using JamSpace.Application.Features.Users.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(    
    Guid UserId,
    string? Bio,
    LocationDto? Location,
    string? ProfilePictureUrl
) : IRequest<UserDto>;