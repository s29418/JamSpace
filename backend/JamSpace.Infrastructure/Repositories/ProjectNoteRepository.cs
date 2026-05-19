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

    public async Task<ProjectNote?> GetByIdAsync(Guid noteId, CancellationToken ct)
    {
        return await NotesWithDetails()
            .FirstOrDefaultAsync(n => n.Id == noteId, ct);
    }

    public async Task<IReadOnlyList<ProjectNote>> GetByProjectIdAsync(Guid projectId, CancellationToken ct)
    {
        return await NotesWithDetails()
            .Where(n => n.ProjectId == projectId)
            .OrderBy(n => n.Status)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProjectNote>> GetByProjectIdAndAudioVersionIdAsync(
        Guid projectId,
        Guid audioVersionId,
        CancellationToken ct)
    {
        return await NotesWithDetails()
            .Where(n => n.ProjectId == projectId && n.AudioVersionId == audioVersionId)
            .OrderBy(n => n.Status)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProjectNote>> GetByAudioVersionIdAsync(Guid audioVersionId, CancellationToken ct)
    {
        return await _db.ProjectNotes
            .Where(n => n.AudioVersionId == audioVersionId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ProjectNote note, CancellationToken ct)
    {
        await _db.ProjectNotes.AddAsync(note, ct);
    }

    public void Remove(ProjectNote note)
    {
        _db.ProjectNotes.Remove(note);
    }

    private IQueryable<ProjectNote> NotesWithDetails()
    {
        return _db.ProjectNotes
            .Include(n => n.AudioVersion)
            .Include(n => n.CreatedBy)
            .Include(n => n.CompletedBy);
    }
}
