using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.Like;

public record LikePostCommand(Guid UserId, Guid PostId) : IRequest<Unit>;