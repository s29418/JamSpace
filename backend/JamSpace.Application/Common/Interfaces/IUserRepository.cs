using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
