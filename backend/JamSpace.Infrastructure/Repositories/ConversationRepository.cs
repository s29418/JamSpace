using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly JamSpaceDbContext _db;
    
    public ConversationRepository(JamSpaceDbContext db) => _db = db;

    public async Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken ct)
    {
        return await _db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == conversationId, ct);
    }

    public async Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Include(c => c.Participants).ThenInclude(p => p.User)
            .Include(c => c.Team)
            .AsNoTracking()
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(ct);
    }

    public async Task<Guid?> GetIdForDirect(string directKey, CancellationToken ct)
    {
        return await _db.Conversations
            .Where(c => c.DirectKey == directKey)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Conversation?> GetForTeam(Guid teamId, CancellationToken ct)
    {
        return await _db.Conversations
            .FirstOrDefaultAsync(c => c.TeamId == teamId, ct);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken ct) 
        => await _db.Conversations.AddAsync(conversation, ct);

    public void Remove(Conversation conversation) => _db.Remove(conversation);
}