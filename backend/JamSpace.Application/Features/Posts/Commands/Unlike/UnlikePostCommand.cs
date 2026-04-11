using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Unlike;

public record UnlikePostCommand(Guid UserId, Guid PostId) : IRequest<Unit>;