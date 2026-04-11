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

    public async Task AddAsync(PostComment comment, CancellationToken ct) => 
        await _db.PostComments
            .AddAsync(comment, ct);

    public async Task Delete(PostComment comment, CancellationToken ct) => 
        await _db.PostComments
            .Where(c => c == comment)
            .ExecuteDeleteAsync(ct);
}