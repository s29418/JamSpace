using JamSpace.Domain.Enums;

namespace JamSpace.Application.Features.Conversations.DTOs;

public class ConversationDetailsDto
{
    public Guid Id { get; init; }
    public ChatType Type { get; init; }
    
    public Guid? TargetUserId { get; init; }
    public Guid? TeamId { get; init; }

    public string DisplayName { get; init; } = default!;
    public string? DisplayPictureUrl { get; init; }

    public List<ConversationParticipantDto> Participants { get; init; } = new();
}