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

        if (!_hasher.Verify(c.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("CurrentPassword", "Current password is incorrect.")
            });
        }

        var (isValid, errors) = _policy.Validate(c.NewPassword);
        if (!isValid)
        {
            throw new ValidationException(errors.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Key, e.Value))
            );
        }

        user.PasswordHash = _hasher.Hash(c.NewPassword);

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}