using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Notifications.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(
    Guid UserId,
    DateTimeOffset? Before,
    int Take) : IRequest<CursorResult<NotificationDto>>;
