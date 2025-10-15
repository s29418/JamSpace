using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Users.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User u) =>
        new(u.Id, u.UserName, u.Bio, u.ProfilePictureUrl, u.Email,
            u.Location is null ? null : new LocationDto(u.Location.City, u.Location.CountryCode));
}