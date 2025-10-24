using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using JamSpace.Infrastructure.Data;

namespace JamSpace.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly JamSpaceDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public UnitOfWork(JamSpaceDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public IUserRepository Users => _userRepository;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
