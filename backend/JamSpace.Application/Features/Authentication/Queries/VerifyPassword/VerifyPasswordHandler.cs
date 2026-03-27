using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using MediatR;

namespace JamSpace.Application.Features.Authentication.Queries.VerifyPassword;

public class VerifyPasswordHandler : IRequestHandler<VerifyPasswordQuery, Unit>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    
    public VerifyPasswordHandler(IUserRepository repo, IPasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }
    
    public async Task<Unit> Handle(VerifyPasswordQuery request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("User not found.");

        if (!_hasher.Verify(request.Password, user.PasswordHash)) 
            throw new ForbiddenAccessException("Invalid password.");
        
        return Unit.Value;
    }
}