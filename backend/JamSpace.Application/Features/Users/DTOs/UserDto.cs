namespace JamSpace.Application.Features.Users.DTOs;

public sealed record UserDto(
    Guid Id, string Username, string? Bio, string? ProfilePictureUrl,
    string? Email, LocationDto? Location
);