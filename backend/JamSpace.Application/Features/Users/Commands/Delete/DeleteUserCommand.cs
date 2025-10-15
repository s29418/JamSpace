using MediatR;

namespace JamSpace.Application.Features.Users.Commands.Delete;

public sealed record DeleteUserCommand(
    Guid UserId,
    byte[] RowVersion 
) : IRequest<Unit>;