namespace JamSpace.Application.Features.Teams.Dtos;

public class TeamMemberDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string? MusicalRole { get; set; }
    public string? UserPictureUrl { get; set; }
}
