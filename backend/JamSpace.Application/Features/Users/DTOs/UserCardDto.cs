namespace JamSpace.Application.Features.Users.Dtos;

public class UserCardDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }

    public string? ProfilePictureUrl { get; init; }

    public string? City { get; init; }
    public string? CountryCode { get; init; }

    public required IReadOnlyList<string> Skills { get; init; }
    public required IReadOnlyList<string> Genres { get; init; }

    public int FollowersCount { get; init; }
    
    public bool IsFollowedByMe { get; init; }
    public bool IsMe { get; init; }
}