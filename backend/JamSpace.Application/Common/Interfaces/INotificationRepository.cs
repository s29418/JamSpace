using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetForUserAsync(
        Guid recipientUserId,
        DateTimeOffset? before,
        int take,
        CancellationToken ct);

    Task<int> CountUnreadAsync(Guid recipientUserId, CancellationToken ct);

    Task AddAsync(Notification notification, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct);
    void Remove(Notification notification);
    void RemoveRange(IEnumerable<Notification> notifications);
}
