using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Persistence;
using JamSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JamSpace.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly JamSpaceDbContext _dbContext;
    public UnitOfWork(JamSpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new UniqueConstraintViolationException("Unique constraint violation.", ex);
        }
    }
    
    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            return sqlEx.Number is 2601 or 2627;
        }

        return false;
    }
}
