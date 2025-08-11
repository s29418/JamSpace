using JamSpace.Application.Features.TeamMembers.DTOs;

namespace JamSpace.Application.Features.Teams.DTOs;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? TeamPictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CreatedByUserName { get; set; } = default!;
    public List<TeamMemberDto> Members { get; set; } = new();
    public string? CurrentUserRole { get; set; }
}
