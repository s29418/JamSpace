using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly JamSpaceDbContext _db;

    public MessageRepository(JamSpaceDbContext db) => _db = db;

    public async Task<Dictionary<Guid, int>> GetUnreadCountsAsync(Guid userId, IReadOnlyList<Guid> conversationIds, CancellationToken ct)
    {
        if (conversationIds.Count == 0)
            return new Dictionary<Guid, int>();

        var query =
            from cp in _db.ConversationParticipants
            join m in _db.Messages
                on cp.ConversationId equals m.ConversationId
            where cp.UserId == userId
                  && conversationIds.Contains(cp.ConversationId)
                  && (cp.LastReadAt == null || m.CreatedAt > cp.LastReadAt)
            group m by m.ConversationId
            into g
            select new { ConversationId = g.Key, Count = g.Count() };

        return await query.ToDictionaryAsync(x => x.ConversationId, x => x.Count, ct);
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, DateTimeOffset? before, int take,
        CancellationToken ct)
    {
        var query = _db.Messages
            .Where(m => m.ConversationId == conversationId);

        if (before is not null)
            query = query.Where(m => m.CreatedAt < before);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Message message, CancellationToken ct) => await _db.Messages.AddAsync(message, ct);
}