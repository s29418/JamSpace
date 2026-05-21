using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly JamSpaceDbContext _db;

    public NotificationRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Notification>> GetForUserAsync(
        Guid recipientUserId,
        DateTimeOffset? before,
        int take,
        CancellationToken ct)
    {
        var query = _db.Notifications
            .Where(n => n.RecipientUserId == recipientUserId);

        if (before is not null)
            query = query.Where(n => n.CreatedAt < before);

        return await query
            .Include(n => n.ActorUser)
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountUnreadAsync(Guid recipientUserId, CancellationToken ct)
    {
        return await _db.Notifications
            .CountAsync(n => n.RecipientUserId == recipientUserId && !n.IsRead, ct);
    }

    public async Task AddAsync(Notification notification, CancellationToken ct)
    {
        await _db.Notifications.AddAsync(notification, ct);
    }

    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct)
    {
        await _db.Notifications.AddRangeAsync(notifications, ct);
    }

    public void Remove(Notification notification)
    {
        _db.Notifications.Remove(notification);
    }

    public void RemoveRange(IEnumerable<Notification> notifications)
    {
        _db.Notifications.RemoveRange(notifications);
    }
}
