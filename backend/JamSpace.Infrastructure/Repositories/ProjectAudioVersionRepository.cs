using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ProjectAudioVersionRepository : IProjectAudioVersionRepository
{
    private readonly JamSpaceDbContext _db;

    public ProjectAudioVersionRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<ProjectAudioVersion?> GetByIdAsync(Guid versionId, CancellationToken ct)
    {
        return await _db.ProjectAudioVersions
            .Include(v => v.CreatedBy)
            .FirstOrDefaultAsync(v => v.Id == versionId, ct);
    }

    public async Task<IReadOnlyList<ProjectAudioVersion>> GetByProjectIdAsync(Guid projectId, CancellationToken ct)
    {
        return await _db.ProjectAudioVersions
            .Include(v => v.CreatedBy)
            .Where(v => v.ProjectId == projectId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ProjectAudioVersion version, CancellationToken ct)
    {
        await _db.ProjectAudioVersions.AddAsync(version, ct);
    }

    public void Remove(ProjectAudioVersion version)
    {
        _db.ProjectAudioVersions.Remove(version);
    }
}
