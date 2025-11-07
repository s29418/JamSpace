using JamSpace.Application.Common.Interfaces;

namespace JamSpace.Application.Common.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
