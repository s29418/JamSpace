using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public class GetUnreadNotificationsCountHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    private readonly INotificationRepository _notifications;

    public GetUnreadNotificationsCountHandler(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken ct)
    {
        return await _notifications.CountUnreadAsync(request.UserId, ct);
    }
}
