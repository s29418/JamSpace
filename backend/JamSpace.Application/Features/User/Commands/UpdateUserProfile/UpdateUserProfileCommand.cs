using JamSpace.Application.Features.User.DTOs;
using MediatR;

namespace JamSpace.Application.Features.User.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(    
    Guid UserId,
    string? Bio,
    LocationDto? Location,
    string? ProfilePictureUrl
) : IRequest<UserDto>;