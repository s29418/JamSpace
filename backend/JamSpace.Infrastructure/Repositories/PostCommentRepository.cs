using JamSpace.Application.Common.Interfaces;
using JamSpace.Domain.Entities;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Repositories;

public class PostCommentRepository : IPostCommentRepository
{
    private readonly JamSpaceDbContext _db;

    public PostCommentRepository(JamSpaceDbContext db)
    {
        _db = db;
    }

    public async Task<PostComment?> GetByIdAsync(Guid commentId, CancellationToken ct)
    {
        return await _db.PostComments
            .FirstOrDefaultAsync(c => c.Id == commentId, ct);
    }

    public async Task AddAsync(PostComment comment, CancellationToken ct) => 
        await _db.PostComments
            .AddAsync(comment, ct);

    public void Delete(PostComment comment) => _db.PostComments.Remove(comment);
}