using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.TeamInvites.DTOs;

public class TeamInviteDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string? TeamName { get; set; } = default!;
    public string? TeamPictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? InvitedByUserName { get; set; } = default!;
    public string? InvitedUserName { get; set; } = default!;
    public Guid? InvitedUserId { get; set; }
    public string? InvitedUserPictureUrl { get; set; }
    public string? Status { get; set; }
}
