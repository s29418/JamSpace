using JamSpace.Application.Features.Notifications.DTOs;
using JamSpace.Domain.Entities;

namespace JamSpace.Application.Features.Notifications.Mappers;

public static class NotificationMapper
{
    public static NotificationDto ToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            ActorUserId = notification.ActorUserId,
            ActorDisplayName = notification.ActorUser?.DisplayName,
            ActorAvatarUrl = notification.ActorUser?.ProfilePictureUrl,
            ConversationId = notification.ConversationId,
            TeamId = notification.TeamId,
            TeamInviteId = notification.TeamInviteId,
            TeamEventId = notification.TeamEventId,
            PostId = notification.PostId,
            ProjectId = notification.ProjectId,
            ProjectNoteId = notification.ProjectNoteId
        };
    }
}
