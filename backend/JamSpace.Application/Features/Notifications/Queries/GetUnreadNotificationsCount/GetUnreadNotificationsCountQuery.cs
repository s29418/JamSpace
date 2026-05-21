using MediatR;

namespace JamSpace.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public record GetUnreadNotificationsCountQuery(Guid UserId) : IRequest<int>;
