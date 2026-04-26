using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly JamSpaceDbContext _db;

    public ProjectRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken ct)
    {
        return await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);
    }

    public async Task<IReadOnlyList<Project>> GetByTeamIdAsync(Guid teamId, CancellationToken ct)
    {
        return await _db.Projects
            .Where(p => p.TeamId == teamId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Project project, CancellationToken ct)
    {
        await _db.Projects.AddAsync(project, ct);
    }
}