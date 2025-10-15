using JamSpace.Application.Common.Exceptions;
using JamSpace.Application.Common.Interfaces;
using JamSpace.Application.Common.Persistence;
using MediatR;

namespace JamSpace.Application.Features.Users.Commands.Delete;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public DeleteUserHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<Unit> Handle(DeleteUserCommand c, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(c.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        if (!user.RowVersion.SequenceEqual(c.RowVersion))
            throw new ConcurrencyException("User was modified by someone else.");

        if (!user.IsDeleted) 
        {
            user.IsDeleted = true;
        }

        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}