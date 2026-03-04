using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ConversationParticipantRepository : IConversationParticipantRepository
{
    private readonly JamSpaceDbContext _db;
    
    public ConversationParticipantRepository(JamSpaceDbContext db) => _db = db;


    public async Task<bool> IsParticipantAsync(Guid conversationId, Guid userId, CancellationToken ct)
    {
        return await _db.ConversationParticipants
            .AnyAsync(c => c.ConversationId == conversationId && c.UserId == userId, ct);
    }

    public async Task<ConversationParticipant?> GetAsync(Guid teamId, Guid userId, CancellationToken ct)
    {
        return await _db.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Where(cp => cp.Conversation!.TeamId == teamId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(ConversationParticipant conversationParticipant, CancellationToken ct) 
        =>  await _db.ConversationParticipants.AddAsync(conversationParticipant, ct);

    public void Remove(ConversationParticipant conversationParticipant) => _db.Remove(conversationParticipant);
}