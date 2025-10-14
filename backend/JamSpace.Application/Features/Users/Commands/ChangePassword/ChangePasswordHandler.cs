using FluentValidation;
using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.ChangePassword;

public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IPasswordPolicy _policy;
    private readonly IUnitOfWork _uow;

    public ChangePasswordHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IPasswordPolicy policy,
        IUnitOfWork uow)
    {
        _users = users;
        _hasher = hasher;
        _policy = policy;
        _uow = uow;
    }

    public async Task<Unit> Handle(ChangePasswordCommand c, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(c.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        if (!_hasher.Verify(user.PasswordHash, c.CurrentPassword))
            throw new ValidationException("Current password is incorrect.");

        var (ok, error) = _policy.Validate(c.NewPassword);
        if (!ok) throw new ValidationException(error ?? "New password doesn't meet policy.");

        user.PasswordHash = _hasher.Hash(c.NewPassword);

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}