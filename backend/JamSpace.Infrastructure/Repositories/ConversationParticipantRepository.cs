using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ConversationParticipantRepository : IConversationParticipantRepository
{
    private readonly JamSpaceDbContext _db;
    
    public ConversationParticipantRepository(JamSpaceDbContext db) => _db = db;

    public async Task AddAsync(ConversationParticipant conversationParticipant, CancellationToken ct) 
        =>  await _db.ConversationParticipants.AddAsync(conversationParticipant, ct);
}