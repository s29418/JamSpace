using System.Runtime.InteropServices.ComTypes;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;

namespace JamSpace.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly JamSpaceDbContext _db;

    public MessageRepository(JamSpaceDbContext db) => _db = db;

    public async Task AddAsync(Message message, CancellationToken ct) => await _db.Messages.AddAsync(message, ct);
}