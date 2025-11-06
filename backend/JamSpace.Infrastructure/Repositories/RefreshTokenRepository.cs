using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly JamSpaceDbContext _db;

    public RefreshTokenRepository(JamSpaceDbContext db)
    {
        _db = db;
    } 

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(ct);
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct) =>
        _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);

    public async Task RotateAsync(RefreshToken oldToken, string newToken, DateTime newExpiry, CancellationToken ct)
    {
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByToken = newToken;

        var rt = new RefreshToken
        {
            UserId = oldToken.UserId,
            Token = newToken,
            ExpiresAt = newExpiry,
            IpAddress = oldToken.IpAddress,
            UserAgent = oldToken.UserAgent
        };
        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(string token, CancellationToken ct)
    {
        var rt = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, ct);
        
        if (rt != null)
        {
            rt.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        var tokens = await _db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);
        
        foreach (var t in tokens) t.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}