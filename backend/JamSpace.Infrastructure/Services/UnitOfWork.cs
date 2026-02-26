using JamSpace.Application.Common.Persistence;
using JamSpace.Infrastructure.Data;

namespace JamSpace.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly JamSpaceDbContext _dbContext;
    public UnitOfWork(JamSpaceDbContext dbContext) => _dbContext = dbContext;

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}
