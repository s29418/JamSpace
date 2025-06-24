namespace JamSpace.Application.Features.Teams.Dtos;

public class TeamInviteDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string InvitedByUserName { get; set; } = default!;
}
