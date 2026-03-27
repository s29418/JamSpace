using JamSpace.Application.Features.UserGenres.DTOs;
using JamSpace.Application.Features.UserSkills.DTOs;

namespace JamSpace.Application.Features.Users.DTOs;

public sealed record UserDto(
    Guid Id,
    string Username, 
    string DisplayName,
    string? Bio, 
    string? ProfilePictureUrl,
    string? Email,
    LocationDto? Location,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowing,
    UserSkillDto[] Skills,
    UserGenreDto[] Genres
);