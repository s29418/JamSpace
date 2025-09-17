using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<Guid?> GetUserIdByUsernameAsync(string username, CancellationToken ct);
    
    Task<bool> ExistsAsync(Guid userId, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
    Task<User> UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(Guid userId, CancellationToken ct);
}