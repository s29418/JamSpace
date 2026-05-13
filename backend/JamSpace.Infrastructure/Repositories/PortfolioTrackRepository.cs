using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class PortfolioTrackRepository : IPortfolioTrackRepository
{
    private readonly JamSpaceDbContext _db;

    public PortfolioTrackRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<PortfolioTrack?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.PortfolioTracks
            .Include(x => x.User)
            .Include(x => x.ExternalAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PortfolioTrack?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct)
    {
        return await _db.PortfolioTracks
            .Include(x => x.User)
            .Include(x => x.ExternalAccount)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<PortfolioTrack>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.PortfolioTracks
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetNextDisplayOrderAsync(Guid userId, CancellationToken ct)
    {
        var maxOrder = await _db.PortfolioTracks
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.DisplayOrder)
            .MaxAsync(ct);

        return (maxOrder ?? -1) + 1;
    }

    public async Task AddAsync(PortfolioTrack track, CancellationToken ct)
    {
        await _db.PortfolioTracks.AddAsync(track, ct);
    }

    public void SoftDelete(PortfolioTrack track, DateTimeOffset deletedAt)
    {
        track.IsDeleted = true;
        track.UpdatedAt = deletedAt;
    }
}
