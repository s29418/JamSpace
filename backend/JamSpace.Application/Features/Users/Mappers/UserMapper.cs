using JamSpace.Application.Features.UserGenres.DTOs;
using JamSpace.Application.Features.Users.DTOs;
using JamSpace.Application.Features.UserSkills.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Users.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User u, bool isSelf, bool isFollowing) =>
        new(u.Id,
            u.UserName, 
            u.Bio, 
            u.ProfilePictureUrl,
            isSelf ? u.Email : null,
            u.Location is null ? null : new LocationDto(u.Location.City, u.Location.CountryCode),
            u.Followers.Count,
            u.Following.Count,
            isFollowing,
            u.UserSkills.Select(skill => new UserSkillDto(
                skill.Skill.Id,
                skill.Skill.Name))
                .ToArray(),
            u.UserGenres.Select(genre => new UserGenreDto(
                    genre.Genre.Id,
                    genre.Genre.Name))
                .ToArray()
            );
}