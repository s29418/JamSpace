using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IPostCommentRepository
{
    Task<PostComment?> GetByIdAsync(Guid commentId, CancellationToken ct);
    Task AddAsync(PostComment comment, CancellationToken ct);
    void Delete(PostComment comment);
}