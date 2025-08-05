using JamSpace.Application.Features.TeamMembers.DTOs;

namespace JamSpace.Application.Features.Teams.Dtos;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? TeamPictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CreatedByUserName { get; set; } = default!;
    public List<TeamMemberDto> Members { get; set; } = new();
}
