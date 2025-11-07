using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Infrastructure.Data;

namespace JamSpace.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly JamSpaceDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public UnitOfWork(JamSpaceDbContext dbContext, IUserRepository userRepository, 
        IRefreshTokenRepository refreshTokenRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public IUserRepository Users => _userRepository;
    public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
