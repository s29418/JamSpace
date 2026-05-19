using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class ProjectNoteRepository : IProjectNoteRepository
{
    private readonly JamSpaceDbContext _db;

    public ProjectNoteRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProjectNote>> GetByAudioVersionIdAsync(Guid audioVersionId, CancellationToken ct)
    {
        return await _db.ProjectNotes
            .Where(n => n.AudioVersionId == audioVersionId)
            .ToListAsync(ct);
    }
}
