using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Notifications.DTOs;
using JamSpace.Application.Features.Notifications.Mappers;
using MediatR;

namespace JamSpace.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, CursorResult<NotificationDto>>
{
    private readonly INotificationRepository _notifications;

    public GetMyNotificationsHandler(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public async Task<CursorResult<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var take = Math.Clamp(request.Take, 1, 50);
        var takePlusOne = take + 1;

        var notifications = await _notifications.GetForUserAsync(
            request.UserId,
            request.Before,
            takePlusOne,
            ct);

        if (notifications.Count == 0)
            return CursorResult<NotificationDto>.Create(Array.Empty<NotificationDto>(), false, null);

        var hasMore = notifications.Count == takePlusOne;
        var pageNotifications = hasMore
            ? notifications.Take(take).ToList()
            : notifications.ToList();

        var nextBefore = pageNotifications.Last().CreatedAt;
        var dtoItems = pageNotifications
            .Select(NotificationMapper.ToDto)
            .ToList();

        return CursorResult<NotificationDto>.Create(dtoItems, hasMore, nextBefore);
    }
}
