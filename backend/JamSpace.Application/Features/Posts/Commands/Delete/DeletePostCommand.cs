using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Delete;

public record DeletePostCommand(Guid PostId, Guid RequestingUserId) : IRequest<Unit>;